using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class InstructionsSet : CecilatorBase, ICode, IAction, ITryCode
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly InstructionContainer instructions;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly Method method;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly ILProcessor processor;

        internal InstructionsSet(BuilderType type, Method method) : base(type)
        {
            this.instructions = new InstructionContainer();
            this.method = method;
            this.processor = method.GetILProcessor();
            this.method.methodDefinition.Body.SimplifyMacros();
        }

        internal InstructionsSet(InstructionsSet instructionsSet, InstructionContainer instructions) : base(instructionsSet.method.DeclaringType)
        {
            this.method = instructionsSet.method;
            this.processor = instructionsSet.processor;
            this.instructions = instructions;
        }

        public Crumb Parameters
        {
            get
            {
                var variableName = "<>params_" + this.method.Identification;
                if (!this.instructions.Variables.Contains(variableName))
                {
                    var objectArrayType = this.method.DeclaringType.Builder.GetType(typeof(object[]));
                    var variable = this.CreateVariable(variableName, objectArrayType);
                    var newInstructions = new List<Instruction>();

                    newInstructions.Add(processor.Create(OpCodes.Ldc_I4, this.method.methodReference.Parameters.Count));
                    newInstructions.Add(processor.Create(OpCodes.Newarr, (objectArrayType.typeReference as ArrayType).ElementType));
                    newInstructions.Add(processor.Create(OpCodes.Stloc, variable.variable));

                    foreach (var parameter in this.method.methodReference.Parameters)
                        newInstructions.AddRange(IlHelper.ProcessParam(parameter, variable.variable));
                    // Insert the call in the beginning of the instruction list
                    this.instructions.Insert(0, newInstructions);
                }

                return new Crumb { CrumbType = CrumbTypes.Parameters, Name = variableName };
            }
        }

        public Crumb This { get { return new Crumb { CrumbType = CrumbTypes.This }; } }

        protected bool RequiresReturn
        {
            get
            {
                return this.instructions.Count > 0 &&
                    this.instructions.Last().OpCode != OpCodes.Rethrow &&
                    this.instructions.Last().OpCode != OpCodes.Throw &&
                    this.instructions.Last().OpCode != OpCodes.Ret;
            }
        }

        public ILocalVariableCode Assign(LocalVariable localVariable) => new LocalVariableInstructionSet(this, localVariable, this.instructions, AssignInstructionType.Store);

        public IFieldCode Assign(Field field)
        {
            if (!field.IsStatic)
            {
                if (field.DeclaringType != this.method.DeclaringType)
                    throw new NotImplementedException();

                this.instructions.Append(processor.Create(OpCodes.Ldarg_0));
            }

            return new FieldInstructionsSet(this, field, this.instructions, AssignInstructionType.Store);
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
            if (method.Parameters.Length != parameters.Length)
                this.LogWarning($"Parameter count of method {method.Name} does not match with the passed parameters. Expected: {method.Parameters.Length}, is: {parameters.Length}");

            if (method.DeclaringType.IsInterface || method.IsAbstract)
                this.LogError($"Use Callvirt to call {method.ToString()}");

            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(this.processor, method.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.Append(inst.Instructions);
            }

            this.instructions.Append(processor.Create(OpCodes.Call, this.moduleDefinition.Import(method.methodReference)));

            if (!method.IsVoid)
                this.StoreCall();

            return new InstructionsSet(this, this.instructions);
        }

        public ICode Callvirt(Method method, params object[] parameters)
        {
            if (method.Parameters.Length != parameters.Length)
                this.LogWarning($"Parameter count of method {method.Name} does not match with the passed parameters. Expected: {method.Parameters.Length}, is: {parameters.Length}");

            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(this.processor, method.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.Append(inst.Instructions);
            }

            this.instructions.Append(processor.Create(OpCodes.Callvirt, this.moduleDefinition.Import(method.methodReference)));

            if (!method.IsVoid)
                this.StoreCall();

            return new InstructionsSet(this, this.instructions);
        }

        public ICode Context(Action<ICode> body)
        {
            body(this);
            return new InstructionsSet(this, this.instructions);
        }

        public Method Copy(Modifiers modifiers, string newName)
        {
            var jumps = new List<Tuple<int, int>>();
            var method = this.method.DeclaringType.typeDefinition.Methods.FirstOrDefault(x => x.Name == newName);

            if (method != null)
                return new Method(this.method.DeclaringType, method);

            var attributes = MethodAttributes.CompilerControlled;

            if (modifiers.HasFlag(Modifiers.Private)) attributes |= MethodAttributes.Private;
            if (this.method.IsStatic) attributes |= MethodAttributes.Static;
            if (modifiers.HasFlag(Modifiers.Public)) attributes |= MethodAttributes.Public;

            method = new MethodDefinition(newName, attributes, this.method.methodReference.ReturnType);

            foreach (var item in this.method.methodReference.Parameters)
                method.Parameters.Add(item);

            foreach (var item in this.method.methodReference.GenericParameters)
                method.GenericParameters.Add(item);

            foreach (var item in this.method.methodDefinition.Body.Variables)
                method.Body.Variables.Add(item);

            method.Body.InitLocals = this.method.methodDefinition.Body.InitLocals;

            this.method.DeclaringType.typeDefinition.Methods.Add(method);
            var methodProcessor = method.Body.GetILProcessor();

            for (int i = 0; i < this.method.methodDefinition.Body.Instructions.Count; i++)
            {
                var item = this.method.methodDefinition.Body.Instructions[i];

                if (item.Operand is Instruction)
                {
                    var operand = item.Operand as Instruction;
                    var index = this.method.methodDefinition.Body.Instructions.IndexOf(operand);
                    jumps.Add(new Tuple<int, int>(i, index));

                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    methodProcessor.Append(instruction);
                }
                else if (item.Operand is CallSite || item.Operand is Instruction[])
                    throw new NotImplementedException($"Unknown operand '{item.Operand.GetType().FullName}'");
                else
                {
                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = item.Operand;
                    methodProcessor.Append(instruction);
                }
            }

            for (int i = 0; i < jumps.Count; i++)
                methodProcessor.Body.Instructions[jumps[i].Item1].Operand = methodProcessor.Body.Instructions[jumps[i].Item2];

            var getInstruction = new Func<Instruction, Instruction>(x =>
            {
                if (x == null)
                    return null;

                var index = this.method.methodDefinition.Body.Instructions.IndexOf(x);
                return method.Body.Instructions[index];
            });

            foreach (var item in this.method.methodDefinition.Body.ExceptionHandlers)
            {
                var handler = new ExceptionHandler(item.HandlerType)
                {
                    CatchType = item.CatchType,
                    FilterStart = getInstruction(item.FilterStart),
                    HandlerEnd = getInstruction(item.HandlerEnd),
                    HandlerStart = getInstruction(item.HandlerStart),
                    TryEnd = getInstruction(item.TryEnd),
                    TryStart = getInstruction(item.TryStart)
                };
                method.Body.ExceptionHandlers.Add(handler);
            }

            method.Body.OptimizeMacros();

            return new Method(this.method.DeclaringType, method);
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
                        var localVariable = this.GetOrCreateReturnVariable();

                        processor.InsertBefore(last, processor.Create(OpCodes.Stloc, localVariable));
                        processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, localVariable));
                        instructionPosition = last.Previous.Previous;
                    }

                    foreach (var item in jumpers)
                        item.Operand = this.instructions.First();
                }
            }

            foreach (var item in this.instructions.Variables)
                processor.Body.Variables.Add(item);

            processor.InsertBefore(instructionPosition, this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;
            this.ReplaceReturns();

            this.method.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
        }

        public IFieldCode Load(Field field)
        {
            if (!field.IsStatic)
                this.instructions.Append(processor.Create(OpCodes.Ldarg_0));

            this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field.fieldRef));
            return this.CreateFieldInstructionSet(field, AssignInstructionType.Load);
        }

        public ILocalVariableCode Load(LocalVariable localVariable)
        {
            switch (localVariable.Index)
            {
                case 0: this.instructions.Append(processor.Create(OpCodes.Ldloc_0)); break;
                case 1: this.instructions.Append(processor.Create(OpCodes.Ldloc_1)); break;
                case 2: this.instructions.Append(processor.Create(OpCodes.Ldloc_2)); break;
                case 3: this.instructions.Append(processor.Create(OpCodes.Ldloc_3)); break;
                default:
                    this.instructions.Append(processor.Create(OpCodes.Ldloc, localVariable.variable));
                    break;
            }

            return this.CreateLocalVariableInstructionSet(localVariable, AssignInstructionType.Load);
        }

        public ICode Load(Crumb crumb)
        {
            var inst = this.AddParameter(this.processor, null, crumb);
            this.instructions.Append(inst.Instructions);

            return this;
        }

        public IFieldCode LoadField(string fieldName)
        {
            if (!this.method.DeclaringType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.DeclaringType}'");

            var field = this.method.DeclaringType.Fields[fieldName];
            return this.Load(field);
        }

        public ILocalVariableCode LoadVariable(string variableName)
        {
            if (!this.instructions.Variables.Contains(variableName))
                throw new KeyNotFoundException($"The local variable with the name '{variableName}' does not exist in '{method.DeclaringType}'");

            var localvariable = this.instructions.Variables[variableName];
            return this.Load(new LocalVariable(this.method.DeclaringType, localvariable));
        }

        public ILocalVariableCode LoadVariable(int variableIndex)
        {
            var localvariable = this.instructions.Variables[variableIndex];
            return this.Load(new LocalVariable(this.method.DeclaringType, localvariable));
        }

        public ICode NewCode() => new InstructionsSet(this.method.DeclaringType, this.method);

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            if (constructor.methodDefinition.Parameters.Count != parameters.Length)
                this.LogWarning($"Parameter count of constructor {constructor.Name} does not match with the passed parameters. Expected: {constructor.methodDefinition.Parameters.Count}, is: {parameters.Length}");

            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.Append(inst.Instructions);
            }

            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        public ICode NewObj(AttributedProperty attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode OriginalBody()
        {
            var newMethod = this.Copy(Modifiers.Private, $"<{this.method.Name}>m__original");

            for (int i = 0; i < this.method.Parameters.Length + (this.method.IsStatic ? 0 : 1); i++)
                this.instructions.Append(processor.Create(OpCodes.Ldarg, i));

            this.instructions.Append(processor.Create(OpCodes.Call, this.moduleDefinition.Import(newMethod.methodReference)));

            return new InstructionsSet(this, this.instructions);
        }

        public ICode Pop()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Pop));
            return this;
        }

        public void Replace()
        {
            this.method.methodDefinition.Body.Instructions.Clear();
            this.method.methodDefinition.Body.Variables.Clear();
            this.method.methodDefinition.Body.ExceptionHandlers.Clear();

            foreach (var item in this.instructions.Variables)
                processor.Body.Variables.Add(item);

            processor.Append(this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;

            this.ReplaceReturns();

            this.method.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
        }

        public ICode Return()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Ret));
            return new InstructionsSet(this, this.instructions);
        }

        public ITry Try(Action<ITryCode> body)
        {
            var markerStart = this.instructions.Last();
            body(this);

            if (this.RequiresReturn)
                this.instructions.Append(this.processor.Create(OpCodes.Ret));

            return new MarkerInstructionSet(this, MarkerType.Try, markerStart ?? this.instructions.First(), this.instructions);
        }

        internal ParamResult AddParameter(ILProcessor processor, TypeReference targetType, object parameter)
        {
            var result = new ParamResult();

            if (parameter == null)
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldnull));
                return result;
            }

            var type = parameter.GetType();

            if (type == typeof(string))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldstr, parameter.ToString()));
                result.Type = this.GetTypeDefinition(typeof(string));
            }
            else if (type == typeof(FieldDefinition))
            {
                var value = parameter as FieldDefinition;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.CreateFieldReference()));
                result.Type = value.FieldType;
            }
            else if (type == typeof(FieldReference))
            {
                var value = parameter as FieldReference;
                var fieldDef = value.Resolve();

                if (!fieldDef.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(fieldDef.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value));
                result.Type = value.FieldType;
            }
            else if (type == typeof(Field))
            {
                var value = parameter as Field;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.fieldRef));
                result.Type = value.fieldRef.FieldType;
            }
            else if (type == typeof(int))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)parameter));
                result.Type = this.GetTypeDefinition(typeof(int));
            }
            else if (type == typeof(uint))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(uint)parameter));
                result.Type = this.GetTypeDefinition(typeof(uint));
            }
            else if (type == typeof(bool))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (bool)parameter ? 1 : 0));
                result.Type = this.GetTypeDefinition(typeof(bool));
            }
            else if (type == typeof(char))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (char)parameter));
                result.Type = this.GetTypeDefinition(typeof(char));
            }
            else if (type == typeof(short))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (short)parameter));
                result.Type = this.GetTypeDefinition(typeof(short));
            }
            else if (type == typeof(ushort))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (ushort)parameter));
                result.Type = this.GetTypeDefinition(typeof(ushort));
            }
            else if (type == typeof(byte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(byte)parameter));
                result.Type = this.GetTypeDefinition(typeof(byte));
            }
            else if (type == typeof(sbyte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(sbyte)parameter));
                result.Type = this.GetTypeDefinition(typeof(sbyte));
            }
            else if (type == typeof(long))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)parameter));
                result.Type = this.GetTypeDefinition(typeof(long));
            }
            else if (type == typeof(ulong))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)(ulong)parameter));
                result.Type = this.GetTypeDefinition(typeof(ulong));
            }
            else if (type == typeof(double))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R8, (double)parameter));
                result.Type = this.GetTypeDefinition(typeof(double));
            }
            else if (type == typeof(float))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, (float)parameter));
                result.Type = this.GetTypeDefinition(typeof(float));
            }
            else if (type == typeof(LocalVariable))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, (parameter as LocalVariable).variable);
                result.Type = value.VariableType;
            }
            else if (type == typeof(VariableDefinition))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, parameter);
                result.Type = value.VariableType;
            }
            else if (type == typeof(Crumb))
            {
                var crumb = parameter as Crumb;

                switch (crumb.CrumbType)
                {
                    case CrumbTypes.Exception:
                    case CrumbTypes.Parameters:
                        if (crumb.Index.HasValue)
                        {
                            if (this.method.methodDefinition.Parameters.Count == 0)
                                throw new ArgumentException($"The method {this.method.Name} does not have any parameters");

                            result.Instructions.Add(processor.Create(OpCodes.Ldarg, this.method.IsStatic ? crumb.Index.Value : crumb.Index.Value + 1));
                        }
                        else
                        {
                            var variable = this.instructions.Variables[crumb.Name];
                            result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                        }
                        break;

                    case CrumbTypes.This:
                        result.Instructions.Add(processor.Create(this.method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else if (type == typeof(BuilderType))
            {
                var bt = parameter as BuilderType;
                result.Instructions.AddRange(this.TypeOf(processor, bt.typeReference));
                result.Type = this.GetTypeDefinition(typeof(Type));
            }
            else if (type == typeof(Method))
            {
                var method = parameter as Method;

                if (targetType.FullName == typeof(IntPtr).FullName)
                    result.Instructions.Add(processor.Create(OpCodes.Ldftn, method.methodReference));
                else
                {
                    // methodof
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.methodReference));
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType.typeReference));
                    result.Instructions.Add(processor.Create(OpCodes.Call,
                        this.moduleDefinition.Import(
                            this.GetTypeDefinition(typeof(System.Reflection.MethodBase))
                                .Methods.FirstOrDefault(x => x.Name == "GetMethodFromHandle" && x.Parameters.Count == 2))));
                }
            }
            else if (type == typeof(ParameterDefinition))
            {
                var value = parameter as ParameterDefinition;
                result.Instructions.Add(processor.Create(OpCodes.Ldarg_S, value));
                result.Type = value.ParameterType;
            }
            else if (parameter is InstructionsSet)
                result.Instructions.AddRange((parameter as InstructionsSet).instructions.ToArray());
            else if (parameter is IEnumerable<Instruction>)
            {
                foreach (var item in parameter as IEnumerable<Instruction>)
                    result.Instructions.Add(item);
            }

            if (result.Type != null && result.Type.Resolve() == null)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));

            if (result.Type != null && result.Type.IsValueType && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));

            return result;
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

        protected virtual IFieldCode CreateFieldInstructionSet(Field field, AssignInstructionType instructionType) => new FieldInstructionsSet(this, field, this.instructions, instructionType);

        protected virtual ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable, AssignInstructionType instructionType) => new LocalVariableInstructionSet(this, localVariable, this.instructions, instructionType);

        protected void InstructionDebug() => this.LogInfo(this.instructions);

        protected ICode NewObj(CustomAttribute attribute)
        {
            foreach (var arg in attribute.ConstructorArguments)
                this.instructions.Append(AttributeParameterToOpCode(this.processor, arg));

            this.instructions.Append(this.processor.Create(OpCodes.Newobj, attribute.Constructor));
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        protected void ReplaceReturns()
        {
            if (this.method.IsVoid)
            {
                var realReturn = this.method.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = this.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = OpCodes.Leave;
                    instruction.Operand = realReturn;
                }
            }
            else
            {
                var returnVariable = this.GetOrCreateReturnVariable();
                var realReturn = this.method.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsLoadLocal() &&
                    !realReturn.Previous.IsLoadField() &&
                    !realReturn.Previous.IsCallOrNew() &&
                    realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    resultJump = true;
                    this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    realReturn = realReturn.Previous;
                }
                else if (realReturn.Previous.IsLoadField() || realReturn.Previous.IsLoadLocal() || realReturn.Previous.OpCode == OpCodes.Ldnull)
                {
                    realReturn = realReturn.Previous;

                    if (realReturn.OpCode == OpCodes.Ldfld || realReturn.OpCode == OpCodes.Ldflda)
                        realReturn = realReturn.Previous;
                }
                else
                    throw new NotImplementedException("Sorry... Not implemented.");

                for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = this.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = OpCodes.Leave;
                    instruction.Operand = realReturn;

                    if (resultJump)
                        this.processor.InsertBefore(instruction, this.processor.Create(OpCodes.Stloc, returnVariable));
                }
            }
        }

        protected virtual void StoreCall()
        {
        }

        private static VariableDefinition AddVariableDefinitionToInstruction(ILProcessor processor, List<Instruction> instructions, object parameter)
        {
            var value = parameter as VariableDefinition;
            var index = value.Index;

            switch (index)
            {
                case 0: instructions.Add(processor.Create(OpCodes.Ldloc_0)); break;
                case 1: instructions.Add(processor.Create(OpCodes.Ldloc_1)); break;
                case 2: instructions.Add(processor.Create(OpCodes.Ldloc_2)); break;
                case 3: instructions.Add(processor.Create(OpCodes.Ldloc_3)); break;
                default:
                    instructions.Add(processor.Create(OpCodes.Ldloc_S, value));
                    break;
            }

            return value;
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

        private VariableDefinition GetOrCreateReturnVariable()
        {
            var variable = this.method.methodDefinition.Body.Variables.FirstOrDefault(x => x.Name == "<>returnValue");

            if (variable != null)
                return variable;

            variable = new VariableDefinition("<>returnValue", this.method.ReturnType.typeReference);
            this.method.methodDefinition.Body.Variables.Add(variable);
            return variable;
        }

        #region Variable

        public LocalVariable CreateVariable(string name, Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(name, method.DeclaringType);

            return this.CreateVariable(name, method.ReturnType);
        }

        public LocalVariable CreateVariable(string name, BuilderType type)
        {
            var existingVariable = this.method.methodDefinition.Body.Variables.FirstOrDefault(x => x.Name == name);

            if (existingVariable != null)
                return new LocalVariable(this.method.type, existingVariable);

            var newVariable = new VariableDefinition(name, this.moduleDefinition.Import(type.typeReference));
            this.instructions.Variables.Add(newVariable);

            return new LocalVariable(this.method.type, newVariable);
        }

        public LocalVariable CreateVariable(Type type)
        {
            var newVariable = new VariableDefinition("<>var_" + CecilatorBase.GenerateName(), this.moduleDefinition.Import(GetTypeDefinition(type)));
            this.instructions.Variables.Add(newVariable);
            return new LocalVariable(this.method.type, newVariable);
        }

        public LocalVariable CreateVariable(Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(method.DeclaringType);

            return this.CreateVariable(method.ReturnType);
        }

        public LocalVariable CreateVariable(BuilderType type)
        {
            var newVariable = new VariableDefinition("<>var_" + CecilatorBase.GenerateName(), this.moduleDefinition.Import(type.typeReference));
            this.instructions.Variables.Add(newVariable);
            return new LocalVariable(this.method.type, newVariable);
        }

        #endregion Variable

        #region Equitable stuff

        public static implicit operator string(InstructionsSet value) => value.ToString();

        public static bool operator !=(InstructionsSet a, InstructionsSet b) => !(a == b);

        public static bool operator ==(InstructionsSet a, InstructionsSet b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is InstructionsSet)
                return this.Equals(obj as InstructionsSet);

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(InstructionsSet other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return object.Equals(other, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.method.methodDefinition.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff

        #region Comparision stuff

        public IIfCode EqualTo(long value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode EqualTo(int value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int32, value).Instructions);

        public IIfCode EqualTo(bool value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Boolean, value).Instructions);

        public IIfCode IsNull() => this.EqualTo(new Instruction[] { this.processor.Create(OpCodes.Ldnull) });

        public IIfCode NotEqualTo(long value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode NotEqualTo(int value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int32, value).Instructions);

        public IIfCode NotEqualTo(bool value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Boolean, value).Instructions);

        private IIfCode EqualTo(IEnumerable<Instruction> instruction)
        {
            var jumpTarget = this.processor.Create(OpCodes.Nop);
            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        private IIfCode NotEqualTo(IEnumerable<Instruction> instruction)
        {
            var jumpTarget = this.processor.Create(OpCodes.Nop);
            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brtrue, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        #endregion Comparision stuff
    }
}