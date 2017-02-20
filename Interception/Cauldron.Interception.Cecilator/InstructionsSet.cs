using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class InstructionsSet : CecilatorBase, ICode, IAction
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected List<Instruction> instructions = new List<Instruction>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ILProcessor processor;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Method method;

        internal InstructionsSet(BuilderType type, Method method) : base(type)
        {
            this.method = method;
            this.processor = method.GetILProcessor();
        }

        internal InstructionsSet(InstructionsSet instructionsSet, IEnumerable<Instruction> instructions) : base(instructionsSet.method.DeclaringType)
        {
            this.method = instructionsSet.method;
            this.processor = instructionsSet.processor;
            this.instructions.AddRange(instructions);
        }

        public ILocalVariableCode Assign(LocalVariable localVariable) => new LocalVariableInstructionSet(this, localVariable, this.instructions);

        public IFieldCode Assign(Field field)
        {
            if (!field.IsStatic)
            {
                if (field.DeclaringType != this.method.DeclaringType)
                    throw new NotImplementedException();

                this.instructions.Add(processor.Create(OpCodes.Ldarg_0));
            }

            return new FieldInstructionsSet(this, field, this.instructions);
        }

        public IFieldCode AssignToField(string fieldName)
        {
            if (!this.method.DeclaringType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.DeclaringType}'");

            var field = this.method.DeclaringType.Fields[fieldName];
            return this.Assign(field);
        }

        public ILocalVariableCode AssignToLocalVariable(int localVariableIndex) => this.Assign(this.method.LocalVariables[localVariableIndex]);

        public ILocalVariableCode AssignToLocalVariable(string localVariableName)
        {
            if (!this.method.LocalVariables.Contains(localVariableName))
                throw new KeyNotFoundException($"The local variable with the name '{localVariableName}' does not exist in '{method.DeclaringType}'");

            return this.Assign(this.method.LocalVariables[localVariableName]);
        }

        public ICode Call(Method method, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(false, this.processor, method.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.AddRange(inst.Instructions);
            }

            this.instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(method.methodReference)));

            if (!method.IsVoid)
                this.StoreCall();

            return new InstructionsSet(this, this.instructions);
        }

        public ICode Callvirt(Method method, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(false, this.processor, method.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.AddRange(inst.Instructions);
            }

            this.instructions.Add(processor.Create(OpCodes.Callvirt, this.moduleDefinition.Import(method.methodReference)));

            if (!method.IsVoid)
                this.StoreCall();

            return new InstructionsSet(this, this.instructions);
        }

        public void Insert(InsertionPosition position)
        {
            Instruction instructionPosition = null;
            if (processor.Body.Instructions.Count == 0)
                processor.Append(processor.Create(OpCodes.Ret));

            if (position == InsertionPosition.Beginning)
            {
                if (this.method.IsCtor)
                    instructionPosition = this.GetCtorBaseOrThisCall(this.method.methodDefinition);
                else
                    instructionPosition = processor.Body.Instructions[0];
            }
            else
            {
                if (this.method.IsCCtor)
                    instructionPosition = processor.Body.Instructions.Last();
                else if (this.method.IsCtor)
                    instructionPosition = this.GetCtorBaseOrThisCall(this.method.methodDefinition);
                else
                {
                    var last = processor.Body.Instructions.Last();
                    var jumpers = processor.Body.Instructions.GetJumpSources(last.Previous);

                    if (last.Previous.IsLoadLocal() || this.method.methodDefinition.ReturnType == this.moduleDefinition.TypeSystem.Void)
                        instructionPosition = last.Previous;
                    else
                    {
                        var isInitialized = this.method.methodDefinition.Body.InitLocals;
                        var localVariable = new VariableDefinition("<>returnValue", this.method.methodReference.ReturnType);
                        this.method.methodDefinition.Body.Variables.Add(localVariable);

                        if (!isInitialized)
                            this.method.methodDefinition.Body.InitLocals = true;

                        processor.InsertBefore(last, processor.Create(OpCodes.Stloc, localVariable));
                        processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, localVariable));
                        instructionPosition = last.Previous.Previous;
                    }

                    foreach (var item in jumpers)
                        item.Operand = this.instructions[0];
                }
            }

            processor.InsertBefore(instructionPosition, this.instructions);
        }

        public IFieldCode Load(Field field)
        {
            if (!this.method.IsStatic)
                this.instructions.Add(processor.Create(OpCodes.Ldarg_0));

            this.instructions.Add(processor.Create(OpCodes.Ldfld, field.fieldRef));
            return new FieldInstructionsSet(this, field, this.instructions);
        }

        public ILocalVariableCode Load(LocalVariable localVariable)
        {
            switch (localVariable.Index)
            {
                case 0: this.instructions.Add(processor.Create(OpCodes.Ldloc_0)); break;
                case 1: this.instructions.Add(processor.Create(OpCodes.Ldloc_1)); break;
                case 2: this.instructions.Add(processor.Create(OpCodes.Ldloc_2)); break;
                case 3: this.instructions.Add(processor.Create(OpCodes.Ldloc_3)); break;
                default:
                    this.instructions.Add(processor.Create(OpCodes.Ldloc, localVariable.variable));
                    break;
            }

            return this.CreateLocalVariableInstructionSet(localVariable);
        }

        public IFieldCode LoadField(string fieldName)
        {
            if (!this.method.DeclaringType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.DeclaringType}'");

            var field = this.method.DeclaringType.Fields[fieldName];
            return this.Load(field);
        }

        public ILocalVariableCode LoadLocalVariable(int variableIndex)
        {
            var localVariable = this.method.LocalVariables[variableIndex];
            return this.Load(localVariable);
        }

        public ILocalVariableCode LoadLocalVariable(string variableName)
        {
            if (!this.method.LocalVariables.Contains(variableName))
                throw new KeyNotFoundException($"The local variable with the name '{variableName}' does not exist in '{method.DeclaringType}'");

            var localvariable = this.method.LocalVariables[variableName];
            return this.Load(localvariable);
        }

        protected IEnumerable<Instruction> AttributeParameterToOpCode(ILProcessor processor, CustomAttributeArgument attributeArgument)
        {
            /*
				- One of the following types: bool, byte, char, double, float, int, long, short, string, sbyte, ushort, uint, ulong.
				- The type object.
				- The type System.Type.
				- An enum type, provided it has public accessibility and the types in which it is nested (if any) also have public accessibility (Section 17.2).
				- Single-dimensional arrays of the above types.
			 */

            if (attributeArgument.Value == null)
                return new Instruction[] { processor.Create(OpCodes.Ldnull) };

            var valueType = attributeArgument.Value.GetType();

            var result = new List<Instruction>();
            if (valueType.IsArray)
            {
                var array = (attributeArgument.Value as IEnumerable).Cast<CustomAttributeArgument>().ToArray();

                result.Add(processor.Create(OpCodes.Ldc_I4, array.Length));
                result.Add(processor.Create(OpCodes.Newarr, this.moduleDefinition.Import(attributeArgument.Type.GetElementType())));
                result.Add(processor.Create(OpCodes.Dup));

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Value == null)
                        return new Instruction[] { processor.Create(OpCodes.Ldnull), processor.Create(OpCodes.Stelem_Ref) };
                    else
                    {
                        var arrayType = array[i].Value.GetType();
                        result.Add(processor.Create(OpCodes.Ldc_I4, i));
                        result.AddRange(this.CreateInstructionsFromAttributeTypes(processor, array[i].Type, arrayType, array[i].Value));

                        if (arrayType.IsValueType && attributeArgument.Type.GetElementType().IsValueType)
                        {
                            if (arrayType == typeof(int) ||
                                arrayType == typeof(uint) ||
                                arrayType.IsEnum)
                                result.Add(processor.Create(OpCodes.Stelem_I4));
                            else if (arrayType == typeof(bool) ||
                                arrayType == typeof(byte) ||
                                arrayType == typeof(sbyte))
                                result.Add(processor.Create(OpCodes.Stelem_I1));
                            else if (arrayType == typeof(short) ||
                                arrayType == typeof(ushort) ||
                                arrayType == typeof(char))
                                result.Add(processor.Create(OpCodes.Stelem_I2));
                            else if (arrayType == typeof(long) ||
                                arrayType == typeof(ulong))
                                result.Add(processor.Create(OpCodes.Stelem_I8));
                            else if (arrayType == typeof(float))
                                result.Add(processor.Create(OpCodes.Stelem_R4));
                            else if (arrayType == typeof(double))
                                result.Add(processor.Create(OpCodes.Stelem_R8));
                        }
                        else
                            result.Add(processor.Create(OpCodes.Stelem_Ref));
                    }
                    if (i < array.Length - 1)
                        result.Add(processor.Create(OpCodes.Dup));
                }
            }
            else
                result.AddRange(this.CreateInstructionsFromAttributeTypes(processor, attributeArgument.Type, valueType, attributeArgument.Value));

            return result;
        }

        protected virtual IFieldCode CreateFieldInstructionSet(Field field) => new FieldInstructionsSet(this, field, this.instructions);

        protected virtual ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable) => new LocalVariableInstructionSet(this, localVariable, this.instructions);

        protected ICode NewObj(CustomAttribute attribute)
        {
            foreach (var arg in attribute.ConstructorArguments)
                this.instructions.AddRange(AttributeParameterToOpCode(this.processor, arg));

            this.instructions.Add(this.processor.Create(OpCodes.Newobj, attribute.Constructor));
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        protected virtual void StoreCall()
        {
        }

        private IEnumerable<Instruction> CreateInstructionsFromAttributeTypes(ILProcessor processor, TypeReference targetType, Type type, object value)
        {
            if (type == typeof(CustomAttributeArgument))
            {
                var attrib = (CustomAttributeArgument)value;
                type = attrib.Value.GetType();
                value = attrib.Value;
            }

            if (type == typeof(string))
                return new Instruction[] { processor.Create(OpCodes.Ldstr, value.ToString()) };

            if (type == typeof(TypeReference))
                return this.TypeOf(processor, value as TypeReference);

            var createInstructionsResult = new List<Instruction>();

            if (type.IsEnum) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)value));
            else if (type == typeof(int)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)value));
            else if (type == typeof(uint)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(uint)value));
            else if (type == typeof(bool)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (bool)value ? 1 : 0));
            else if (type == typeof(char)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (char)value));
            else if (type == typeof(short)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (short)value));
            else if (type == typeof(ushort)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (ushort)value));
            else if (type == typeof(byte)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(byte)value));
            else if (type == typeof(sbyte)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(sbyte)value));
            else if (type == typeof(long)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I8, (long)value));
            else if (type == typeof(ulong)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I8, (long)(ulong)value));
            else if (type == typeof(double)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R8, (double)value));
            else if (type == typeof(float)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R4, (float)value));

            if (type.IsValueType && !targetType.IsValueType)
                createInstructionsResult.Add(processor.Create(OpCodes.Box, type.IsEnum ?
                   this.moduleDefinition.Import(Enum.GetUnderlyingType(type)) : this.moduleDefinition.Import(this.GetTypeDefinition(type))));

            return createInstructionsResult;
        }

        private Instruction GetCtorBaseOrThisCall(MethodDefinition methodDefinition)
        {
            // public MyClass() : base()
            var ctorCall = methodDefinition.Body.Instructions.FirstOrDefault(inst =>
            {
                if (inst.OpCode != OpCodes.Call)
                    return false;

                if (!(inst.Operand is MethodReference))
                    return false;

                var methodRef = inst.Operand as MethodReference;

                if (methodRef.Name != ".ctor")
                    return false;

                if (methodRef.DeclaringType.FullName == methodDefinition.DeclaringType.BaseType.FullName)
                    return true;

                return false;
            });

            if (ctorCall != null)
                return ctorCall;

            // public MyClass() : this()
            return methodDefinition.Body.Instructions.FirstOrDefault(inst =>
            {
                if (inst.OpCode != OpCodes.Call)
                    return false;

                if (!(inst.Operand is MethodReference))
                    return false;

                var methodRef = inst.Operand as MethodReference;

                if (methodRef.Name != ".ctor")
                    return false;

                if (methodRef.DeclaringType.FullName == methodDefinition.DeclaringType.FullName)
                    return true;

                return false;
            });
        }

        #region Equitable stuff

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            return object.Equals(obj, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.method.methodDefinition.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff
    }
}