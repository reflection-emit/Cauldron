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
            this.instructions = new InstructionContainer(method.methodDefinition.Body.Variables);
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

        public ICode As(BuilderType type)
        {
            this.instructions.Append(this.processor.Create(OpCodes.Isinst, type.typeReference));
            return this;
        }

        public ILocalVariableCode Assign(LocalVariable localVariable) => new LocalVariableInstructionSet(this, localVariable, this.instructions, AssignInstructionType.Store);

        public IFieldCode Assign(Field field)
        {
            if (!field.IsStatic)
            {
                if (field.DeclaringType == this.method.DeclaringType)
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

        public ILocalVariableCode AssignToLocalVariable(int localVariableIndex) => this.Assign(new LocalVariable(this.method.DeclaringType, this.instructions.Variables[localVariableIndex]));

        public ILocalVariableCode AssignToLocalVariable(string localVariableName)
        {
            if (!this.instructions.Variables.Contains(localVariableName))
                throw new KeyNotFoundException($"The local variable with the name '{localVariableName}' does not exist in '{method.DeclaringType}'");

            return this.Assign(new LocalVariable(this.method.DeclaringType, this.instructions.Variables[localVariableName]));
        }

        public ICode Call(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Call, parameters);

        public ICode Call(Crumb instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Call(Field instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Call(LocalVariable instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Callvirt(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(Crumb instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(Field instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(LocalVariable instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Context(Action<ICode> body)
        {
            body(this);
            return this;
        }

        public Method Copy(Modifiers modifiers, string newName)
        {
            var method = this.method.DeclaringType.typeDefinition.Methods.Get(newName);

            if (method != null)
                return new Method(this.method.DeclaringType, method);

            var jumps = new List<Tuple<int, int>>();
            var attributes = MethodAttributes.CompilerControlled;

            if (modifiers.HasFlag(Modifiers.Private)) attributes |= MethodAttributes.Private;
            if (this.method.IsStatic) attributes |= MethodAttributes.Static;
            if (modifiers.HasFlag(Modifiers.Public)) attributes |= MethodAttributes.Public;
            if (modifiers.HasFlag(Modifiers.Overrrides)) attributes |= MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot;

            method = new MethodDefinition(newName, attributes, this.method.methodReference.ReturnType);

            foreach (var item in this.method.methodReference.Parameters)
                method.Parameters.Add(item);

            foreach (var item in this.method.methodReference.GenericParameters)
                method.GenericParameters.Add(item);

            foreach (var item in this.method.methodDefinition.Body.Variables)
                method.Body.Variables.Add(new VariableDefinition(item.Name, item.VariableType));

            method.Body.InitLocals = this.method.methodDefinition.Body.InitLocals;

            this.method.DeclaringType.typeDefinition.Methods.Add(method);
            var methodProcessor = method.Body.GetILProcessor();

            var instructionAction = new Func<Instruction, int, Instruction>((item, i) =>
            {
                var operand = item.Operand as Instruction;
                var index = this.method.methodDefinition.Body.Instructions.IndexOf(operand?.Offset ?? item.Offset);
                jumps.Add(new Tuple<int, int>(i, index));

                var instruction = methodProcessor.Create(OpCodes.Nop);
                instruction.OpCode = item.OpCode;
                return instruction;
            });

            for (int i = 0; i < this.method.methodDefinition.Body.Instructions.Count; i++)
            {
                var item = this.method.methodDefinition.Body.Instructions[i];

                if (item.Operand is Instruction)
                    methodProcessor.Append(instructionAction(item, i));
                else if (item.Operand is Instruction[])
                {
                    var instructions = item.Operand as Instruction[];
                    var newInstructions = new Instruction[instructions.Length];
                    for (int x = 0; x < instructions.Length; x++)
                        newInstructions[x] = instructionAction(instructions[x], i);

                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = newInstructions;

                    methodProcessor.Append(instruction);
                }
                //else if (item.Operand is CallSite )
                //    throw new NotImplementedException($"Unknown operand '{item.OpCode.ToString()}' '{item.Operand.GetType().FullName}'");
                else
                {
                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = item.Operand;
                    methodProcessor.Append(instruction);

                    // Set the correct variable def if required
                    var variable = instruction.Operand as VariableDefinition;
                    if (variable != null)
                        instruction.Operand = method.Body.Variables[variable.Index];
                }
            }

            foreach (var j in jumps.GroupBy(x => x.Item1))
            {
                if (methodProcessor.Body.Instructions[j.Key].OpCode == OpCodes.Switch)
                {
                    var instructions = new List<Instruction>();

                    foreach (var switches in j)
                        instructions.Add(methodProcessor.Body.Instructions[switches.Item2]);

                    methodProcessor.Body.Instructions[j.Key].Operand = instructions.ToArray();
                }
                else
                    methodProcessor.Body.Instructions[j.Key].Operand = methodProcessor.Body.Instructions[j.First().Item2];
            }

            var getInstruction = new Func<Instruction, Instruction>(x =>
            {
                if (x == null)
                    return null;

                var index = this.method.methodDefinition.Body.Instructions.IndexOf(x.Offset);
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

        public ICode Dup()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Dup));
            return this;
        }

        public ICode For(LocalVariable array, Action<ICode, LocalVariable> action)
        {
            var item = this.CreateVariable(array.Type.ChildType);
            var indexer = this.CreateVariable(typeof(int));
            var lengthCheck = processor.Create(OpCodes.Ldloc, indexer.variable);

            // var i = 0;
            this.instructions.Append(processor.Create(OpCodes.Ldc_I4_0));
            this.instructions.Append(processor.Create(OpCodes.Stloc, indexer.variable));
            this.instructions.Append(processor.Create(OpCodes.Br, lengthCheck));

            var start = processor.Create(OpCodes.Nop);
            this.instructions.Append(start);

            this.instructions.Append(processor.Create(OpCodes.Ldloc, array.variable));
            this.instructions.Append(processor.Create(OpCodes.Ldloc, indexer.variable));
            this.instructions.Append(processor.Create(OpCodes.Ldelem_Ref)); // TODO - cases for primitiv types
            this.instructions.Append(processor.Create(OpCodes.Stloc, item.variable));

            action(this, item);

            // i++
            this.instructions.Append(processor.Create(OpCodes.Ldloc, indexer.variable));
            this.instructions.Append(processor.Create(OpCodes.Ldc_I4_1));
            this.instructions.Append(processor.Create(OpCodes.Add));
            this.instructions.Append(processor.Create(OpCodes.Stloc, indexer.variable));

            // i < array.Length
            this.instructions.Append(lengthCheck);
            this.instructions.Append(processor.Create(OpCodes.Ldloc, array.variable));
            this.instructions.Append(processor.Create(OpCodes.Ldlen));
            this.instructions.Append(processor.Create(OpCodes.Conv_I4));
            this.instructions.Append(processor.Create(OpCodes.Clt));
            this.instructions.Append(processor.Create(OpCodes.Brtrue, start));

            return this;
        }

        public Crumb GetParameter(int index)
        {
            return new Crumb
            {
                CrumbType = CrumbTypes.Parameters,
                Index = index
            };
        }

        public Crumb GetParametersArray()
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

        public LocalVariable GetReturnVariable() => new LocalVariable(this.method.type, this.GetOrCreateReturnVariable());

        public void Insert(InsertionAction action, Position position)
        {
            for (int i = processor.Body.Variables.Count; i < this.instructions.Variables.Count; i++)
                processor.Body.Variables.Add(this.instructions.Variables[i]);

            if (action == InsertionAction.After)
                processor.InsertAfter(position.instruction, this.instructions);
            else if (action == InsertionAction.Before)
                processor.InsertBefore(position.instruction, this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.ReplaceReturns();
            this.CleanLocalVariableList();
            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;

            this.method.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
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

                    if (!last.Previous.IsLoadLocal() && this.method.methodDefinition.ReturnType.FullName != "System.Void")
                    {
                        var isInitialized = this.method.methodDefinition.Body.InitLocals;
                        var localVariable = this.GetOrCreateReturnVariable();

                        processor.InsertBefore(last, processor.Create(OpCodes.Stloc, localVariable));
                        processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, localVariable));
                    }

                    instructionPosition = last.Previous;

                    foreach (var item in jumpers)
                        item.Operand = this.instructions.First();
                }
            }

            for (int i = processor.Body.Variables.Count; i < this.instructions.Variables.Count; i++)
                processor.Body.Variables.Add(this.instructions.Variables[i]);

            processor.InsertBefore(instructionPosition, this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.ReplaceReturns();

            this.CleanLocalVariableList();
            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;
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
            if (localVariable.variable.VariableType.IsValueType)
                this.instructions.Append(processor.Create(OpCodes.Ldloca, localVariable.variable));
            else
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

        public ICode Load(Crumb crumb) => this.Load(parameter: crumb);

        public ICode Load(object parameter)
        {
            var inst = this.AddParameter(this.processor, null, parameter);
            this.instructions.Append(inst.Instructions);

            if (parameter != null && parameter is Crumb && (parameter as Crumb).UnPackArray)
            {
                var crumbArray = parameter as Crumb;
                this.instructions.Append(this.AddParameter(this.processor, null, crumbArray.UnPackArrayIndex).Instructions);
                this.instructions.Append(this.processor.Create(OpCodes.Ldelem_Ref));
            }

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

        public ICode Newarr(BuilderType type, int size)
        {
            this.instructions.Append(this.AddParameter(this.processor, null, size).Instructions);
            this.instructions.Append(this.processor.Create(OpCodes.Newarr, this.moduleDefinition.Import(type.typeReference)));

            return this;
        }

        public ICode NewCode() => new InstructionsSet(this, this.instructions.Clone());

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb && (parameters[0] as Crumb).UnPackArray)
            {
                var ctorParameters = constructor.methodDefinition.Parameters;
                for (int i = 0; i < ctorParameters.Count; i++)
                {
                    this.instructions.Append(this.AddParameter(this.processor, null, parameters[0]).Instructions);
                    this.instructions.Append(this.AddParameter(this.processor, null, i).Instructions);
                    this.instructions.Append(this.processor.Create(OpCodes.Ldelem_Ref));
                    var paramResult = new ParamResult { Type = this.moduleDefinition.TypeSystem.Object };
                    this.CastOrBoxValues(this.processor, ctorParameters[i].ParameterType, paramResult, ctorParameters[i].ParameterType.Resolve());
                    this.instructions.Append(paramResult.Instructions);
                }
            }
            else
            {
                if (constructor.methodDefinition.Parameters.Count != parameters.Length)
                    this.LogWarning($"Parameter count of constructor {constructor.Name} does not match with the passed parameters. Expected: {constructor.methodDefinition.Parameters.Count}, is: {parameters.Length}");

                for (int i = 0; i < parameters.Length; i++)
                {
                    var inst = this.AddParameter(this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                    this.instructions.Append(inst.Instructions);
                }
            }

            //var ctor = constructor.methodReference;

            //if(ctor.ContainsGenericParameter && !ctor.IsGenericInstance)
            //    ctor = ctor.MakeHostInstanceGeneric(ctor)

            //foreach (var item in constructor.methodReference)
            //    this.LogInfo(item);

            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.StoreCall();
            return this;
        }

        public ICode NewObj(AttributedProperty attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedType attribute) => this.NewObj(attribute.customAttribute);

        public ICode OriginalBody()
        {
            var newMethod = this.Copy(Modifiers.Private, $"<{this.method.Name}>m__original");

            for (int i = 0; i < this.method.Parameters.Length + (this.method.IsStatic ? 0 : 1); i++)
                this.instructions.Append(processor.Create(OpCodes.Ldarg, i));

            this.instructions.Append(processor.Create(OpCodes.Call, this.moduleDefinition.Import(newMethod.methodReference)));

            return this;
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

            processor.Append(this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.ReplaceReturns();

            foreach (var item in this.instructions.Variables)
                processor.Body.Variables.Add(item);

            this.CleanLocalVariableList();
            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;

            this.method.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
        }

        public ICode Return()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Ret));
            return this;
        }

        public ICode StoreElement(BuilderType arrayType, object element, int index)
        {
            this.instructions.Append(this.AddParameter(this.processor, null, index).Instructions);
            this.instructions.Append(this.AddParameter(this.processor, arrayType.typeReference, element).Instructions);
            this.instructions.Append(processor.Create(OpCodes.Stelem_Ref));

            return this;
        }

        public ICode StoreLocal(LocalVariable localVariable)
        {
            var localStore = new LocalVariableInstructionSet(this, localVariable, this.instructions, AssignInstructionType.Store);
            localStore.StoreCall();
            return this;
        }

        public ICode ThrowNew(Type exception, string message)
        {
            this.instructions.Append(processor.Create(OpCodes.Ldstr, message));
            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(this.moduleDefinition.Import(exception).GetMethodReference(".ctor", new Type[] { typeof(string) }))));
            this.instructions.Append(processor.Create(OpCodes.Throw));
            return this;
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
            var targetDef = targetType?.Resolve();

            if (parameter == null)
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldnull));
                return result;
            }

            var type = parameter.GetType();

            if (type == typeof(string))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldstr, parameter.ToString()));
                result.Type = this.moduleDefinition.TypeSystem.String;
            }
            else if (type == typeof(FieldDefinition))
            {
                var value = parameter as FieldDefinition;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                if (value.FieldType.IsValueType && targetType == null)
                    result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, value.CreateFieldReference()));
                else
                    result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.CreateFieldReference()));
                result.Type = value.FieldType;
            }
            else if (type == typeof(FieldReference))
            {
                var value = parameter as FieldReference;
                var fieldDef = value.Resolve();

                if (!fieldDef.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                if (value.FieldType.IsValueType && targetType == null)
                    result.Instructions.Add(processor.Create(fieldDef.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, value));
                else
                    result.Instructions.Add(processor.Create(fieldDef.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value));
                result.Type = value.FieldType;
            }
            else if (type == typeof(Field))
            {
                var value = parameter as Field;

                if (!value.IsStatic)
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                if (value.FieldType.IsValueType && targetType == null)
                    result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, value.fieldRef));
                else
                    result.Instructions.Add(processor.Create(value.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, value.fieldRef));
                result.Type = value.fieldRef.FieldType;
            }
            else if (type == typeof(int))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Int32;
            }
            else if (type == typeof(uint))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(uint)parameter));
                result.Type = this.moduleDefinition.TypeSystem.UInt32;
            }
            else if (type == typeof(bool))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (bool)parameter ? 1 : 0));
                result.Type = this.moduleDefinition.TypeSystem.Boolean;
            }
            else if (type == typeof(char))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (char)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Char;
            }
            else if (type == typeof(short))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (short)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Int16;
            }
            else if (type == typeof(ushort))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (ushort)parameter));
                result.Type = this.moduleDefinition.TypeSystem.UInt16;
            }
            else if (type == typeof(byte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(byte)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Byte;
            }
            else if (type == typeof(sbyte))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)(sbyte)parameter));
                result.Type = this.moduleDefinition.TypeSystem.SByte;
            }
            else if (type == typeof(long))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Int64;
            }
            else if (type == typeof(ulong))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, (long)(ulong)parameter));
                result.Type = this.moduleDefinition.TypeSystem.UInt64;
            }
            else if (type == typeof(double))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R8, (double)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Double;
            }
            else if (type == typeof(float))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, (float)parameter));
                result.Type = this.moduleDefinition.TypeSystem.Single;
            }
            else if (type == typeof(IntPtr))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, (int)parameter));
                result.Type = this.moduleDefinition.TypeSystem.IntPtr;
            }
            else if (type == typeof(UIntPtr))
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, (int)parameter));
                result.Type = this.moduleDefinition.TypeSystem.UIntPtr;
            }
            else if (type == typeof(LocalVariable))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, (parameter as LocalVariable).variable);
                result.Type = value.VariableType;
            }
            else if (type == typeof(VariableDefinition))
            {
                var value = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, parameter);
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
                            result.Type = this.moduleDefinition.Import(this.method.methodDefinition.Parameters[crumb.Index.Value].ParameterType);
                        }
                        else
                        {
                            var variable = this.instructions.Variables[crumb.Name];
                            result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                            result.Type = this.moduleDefinition.Import(variable.VariableType);
                        }
                        break;

                    case CrumbTypes.This:
                        result.Instructions.Add(processor.Create(this.method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                        result.Type = this.method.DeclaringType.typeReference;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            else if (type == typeof(TypeReference) || type == typeof(TypeDefinition))
            {
                var bt = parameter as TypeReference;
                result.Instructions.AddRange(this.TypeOf(processor, bt));
                result.Type = this.moduleDefinition.Import(typeof(Type));
            }
            else if (type == typeof(BuilderType))
            {
                var bt = parameter as BuilderType;
                result.Instructions.AddRange(this.TypeOf(processor, bt.typeReference));
                result.Type = this.moduleDefinition.Import(typeof(Type));
            }
            else if (type == typeof(Method))
            {
                var method = parameter as Method;

                if (targetType.FullName == typeof(IntPtr).FullName)
                {
                    result.Instructions.Add(processor.Create(OpCodes.Ldftn, method.methodReference));
                    result.Type = this.moduleDefinition.TypeSystem.IntPtr;
                }
                else
                {
                    var methodBaseRef = this.moduleDefinition.Import(typeof(System.Reflection.MethodBase));
                    // methodof
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.methodReference));
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType.typeReference));
                    result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(methodBaseRef.Resolve().Methods.FirstOrDefault(x => x.Name == "GetMethodFromHandle" && x.Parameters.Count == 2))));

                    result.Type = methodBaseRef;
                }
            }
            else if (type == typeof(ParameterDefinition))
            {
                var value = parameter as ParameterDefinition;
                result.Instructions.Add(processor.Create(OpCodes.Ldarg, value));
                result.Type = value.ParameterType;
            }
            else if (type == typeof(ParameterReference))
            {
                var value = parameter as ParameterReference;
                result.Instructions.Add(processor.Create(OpCodes.Ldarg, value.Index));
                result.Type = value.ParameterType;
            }
            else if (parameter is InstructionsSet)
            {
                var instruction = parameter as InstructionsSet;

                if (object.ReferenceEquals(instruction, this))
                    throw new NotSupportedException("Nope... Not gonna work... Use NewCode() if you want to pass an instructions set as parameters.");

                result.Instructions.AddRange(instruction.instructions.ToArray());
            }
            else if (parameter is IEnumerable<Instruction>)
            {
                foreach (var item in parameter as IEnumerable<Instruction>)
                    result.Instructions.Add(item);
            }

            if (result.Type == null || targetType == null || result.Type.FullName == targetType.FullName)
                return result;

            CastOrBoxValues(processor, targetType, result, targetDef);

            return result;
        }

        internal void CastOrBoxValues(ILProcessor processor, TypeReference targetType, ParamResult result, TypeDefinition targetDef)
        {
            // TODO - adds additional checks for not resolved generics
            if (targetDef == null) /* This happens if the target type is a generic */
                result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            else if ((targetDef.FullName == typeof(string).FullName || result.Type.FullName == typeof(object).FullName || targetDef.IsInterface) && !targetType.IsValueType && !targetType.IsArray && !targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                result.Instructions.Add(processor.Create(OpCodes.Isinst, this.moduleDefinition.Import(targetType)));
            else if (targetDef.IsEnum)
            {
                result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                result.Instructions.AddRange(this.TypeOf(processor, this.moduleDefinition.Import(targetType)));
                result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
                result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            }
            else if (result.Type.FullName == typeof(object).FullName && (targetType.IsArray || targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1")))
            {
                var childType = this.moduleDefinition.GetChildrenType(targetType);
                var castMethod = this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(System.Linq.Enumerable)).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(null, childType));
                var toArrayMethod = this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(System.Linq.Enumerable)).GetMethodReference("ToArray", 1).MakeGeneric(null, childType));

                result.Instructions.Add(processor.Create(OpCodes.Isinst, this.moduleDefinition.Import(typeof(IEnumerable))));
                result.Instructions.Add(processor.Create(OpCodes.Call, castMethod));

                if (targetType.IsArray)
                    result.Instructions.Add(processor.Create(OpCodes.Call, toArrayMethod));
            }
            else if (result.Type.FullName == typeof(object).FullName && targetDef.IsValueType)
            {
                if (targetDef.FullName == typeof(int).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(uint).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToUInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(bool).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToBoolean", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(byte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(char).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToChar", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(DateTime).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToDateTime", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(decimal).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToDecimal", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(double).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToDouble", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(short).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(long).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToInt64", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(sbyte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToSByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(float).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToSingle", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ushort).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToUInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ulong).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.Import(this.moduleDefinition.Import(typeof(Convert)).GetMethodReference("ToUInt64", new Type[] { typeof(object) }))));
                else result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            }
            else if ((result.Type.Resolve() == null || result.Type.IsValueType) && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName != result.Type.FullName && this.AreReferenceAssignable(targetType, this.moduleDefinition.Import(result.Type)))
                result.Instructions.Add(processor.Create(OpCodes.Castclass, this.moduleDefinition.Import(result.Type)));
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

                if (array.Length > 0)
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

        protected void CleanLocalVariableList()
        {
            var usedVariables = new List<Tuple<Instruction, int>>();

            for (int i = 0; i < this.method.methodDefinition.Body.Instructions.Count; i++)
            {
                var instruction = this.method.methodDefinition.Body.Instructions[i];

                if (!instruction.IsLoadLocal() && !instruction.IsStoreLocal())
                    continue;

                if (instruction.OpCode == OpCodes.Ldloc ||
                    instruction.OpCode == OpCodes.Ldloca ||
                    instruction.OpCode == OpCodes.Ldloca_S ||
                    instruction.OpCode == OpCodes.Stloc ||
                    instruction.OpCode == OpCodes.Stloc_S)
                {
                    usedVariables.Add(new Tuple<Instruction, int>(instruction, (instruction.Operand as VariableDefinition).Index));
                }
                else if (instruction.OpCode == OpCodes.Ldloc_0 || instruction.OpCode == OpCodes.Stloc_0) usedVariables.Add(new Tuple<Instruction, int>(instruction, 0));
                else if (instruction.OpCode == OpCodes.Ldloc_1 || instruction.OpCode == OpCodes.Stloc_1) usedVariables.Add(new Tuple<Instruction, int>(instruction, 1));
                else if (instruction.OpCode == OpCodes.Ldloc_2 || instruction.OpCode == OpCodes.Stloc_2) usedVariables.Add(new Tuple<Instruction, int>(instruction, 2));
                else if (instruction.OpCode == OpCodes.Ldloc_3 || instruction.OpCode == OpCodes.Stloc_3) usedVariables.Add(new Tuple<Instruction, int>(instruction, 3));
            }

            var usedVariablesOrdered = usedVariables.OrderBy(x => x.Item2).ToArray();

            var variableList = new List<VariableDefinition>();

            for (int i = 0; i < usedVariablesOrdered.Length; i++)
            {
                var variable = this.method.methodDefinition.Body.Variables[usedVariablesOrdered[i].Item2];

                if (!variableList.Contains(variable))
                    variableList.Add(variable);

                if (usedVariablesOrdered[i].Item1.OpCode != OpCodes.Ldloc &&
                    usedVariablesOrdered[i].Item1.OpCode != OpCodes.Ldloca &&
                    usedVariablesOrdered[i].Item1.OpCode != OpCodes.Ldloca_S &&
                    usedVariablesOrdered[i].Item1.OpCode != OpCodes.Stloc &&
                    usedVariablesOrdered[i].Item1.OpCode != OpCodes.Stloc_S)
                    usedVariablesOrdered[i].Item1.Operand = variableList.Count - 1;
            }

            this.method.methodDefinition.Body.Variables.Clear();

            for (int i = 0; i < variableList.Count; i++)
                this.method.methodDefinition.Body.Variables.Add(variableList[i]);
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
            return this;
        }

        protected void ReplaceReturns()
        {
            if (instructions.Count == 0)
                return;

            if (this.method.IsVoid || this.instructions.Last().OpCode != OpCodes.Ret)
            {
                var realReturn = this.method.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = this.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = this.IsInclosedInHandlers(instruction) ? OpCodes.Leave : OpCodes.Br;
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
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    this.processor.InsertBefore(realReturn, this.AddParameter(this.processor, this.method.ReturnType.typeReference, returnVariable).Instructions);

                    realReturn = realReturn.Previous;
                }
                else if (realReturn.Previous.IsLoadField() || realReturn.Previous.IsLoadLocal() || realReturn.Previous.OpCode == OpCodes.Ldnull)
                {
                    realReturn = realReturn.Previous;

                    // Think twice before removing this ;)
                    if (realReturn.OpCode == OpCodes.Ldfld || realReturn.OpCode == OpCodes.Ldflda)
                        realReturn = realReturn.Previous;
                }
                else if (realReturn.Previous.OpCode == OpCodes.Call || realReturn.Previous.OpCode == OpCodes.Callvirt || realReturn.Previous.OpCode == OpCodes.Calli)
                    realReturn = realReturn.Previous;
                else
                    throw new NotImplementedException("Sorry... Not implemented.");

                for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = this.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = this.IsInclosedInHandlers(instruction) ? OpCodes.Leave : OpCodes.Br;
                    instruction.Operand = realReturn;

                    if (resultJump)
                        this.processor.InsertBefore(instruction, this.processor.Create(OpCodes.Stloc, returnVariable));
                }
            }
        }

        protected virtual void StoreCall()
        {
        }

        private static VariableDefinition AddVariableDefinitionToInstruction(ILProcessor processor, List<Instruction> instructions, TypeReference targetType, object parameter)
        {
            var value = parameter as VariableDefinition;
            var index = value.Index;

            if (value.VariableType.IsValueType && targetType == null)
                instructions.Add(processor.Create(OpCodes.Ldloca, value));
            else
                switch (index)
                {
                    case 0: instructions.Add(processor.Create(OpCodes.Ldloc_0)); break;
                    case 1: instructions.Add(processor.Create(OpCodes.Ldloc_1)); break;
                    case 2: instructions.Add(processor.Create(OpCodes.Ldloc_2)); break;
                    case 3: instructions.Add(processor.Create(OpCodes.Ldloc_3)); break;
                    default:
                        instructions.Add(processor.Create(OpCodes.Ldloc, value));
                        break;
                }

            return value;
        }

        private ICode CallInternal(object instance, Method method, OpCode opcode, params object[] parameters)
        {
            if (instance != null)
                this.instructions.Append(this.AddParameter(this.processor, null, instance).Instructions);

            if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb && (parameters[0] as Crumb).UnPackArray)
            {
                var methodParameters = method.methodDefinition.Parameters;
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    this.instructions.Append(this.AddParameter(this.processor, null, parameters[0]).Instructions);
                    this.instructions.Append(this.AddParameter(this.processor, null, i).Instructions);
                    this.instructions.Append(this.processor.Create(OpCodes.Ldelem_Ref));
                    var paramResult = new ParamResult { Type = this.moduleDefinition.TypeSystem.Object };
                    this.CastOrBoxValues(this.processor, methodParameters[i].ParameterType, paramResult, methodParameters[i].ParameterType.Resolve());
                    this.instructions.Append(paramResult.Instructions);
                }
            }
            else
            {
                if (method.Parameters.Length != parameters.Length)
                    this.LogWarning($"Parameter count of method {method.Name} does not match with the passed parameters. Expected: {method.Parameters.Length}, is: {parameters.Length}");

                if ((method.DeclaringType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli)
                    opcode = OpCodes.Callvirt;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.DeclaringType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    var inst = this.AddParameter(this.processor, this.moduleDefinition.Import(parameterType), parameters[i]);
                    this.instructions.Append(inst.Instructions);
                }
            }

            this.instructions.Append(processor.Create(opcode, this.moduleDefinition.Import(method.methodReference)));

            if (!method.IsVoid)
                this.StoreCall();

            return this;
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

            if (type == typeof(TypeReference) || type == typeof(TypeDefinition))
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
            var variable = this.instructions.Variables.FirstOrDefault(x => x.Name == "<>returnValue");

            if (variable != null)
                return variable;

            //if (this.method.methodDefinition.Body.Instructions.Count > 1 && this.method.methodDefinition.Body.Instructions.Last().Previous.IsLoadLocal())
            //{
            //    return this.method.methodDefinition.Body.Instructions.Last().Previous.Operand as VariableDefinition ??
            //        (this.method.methodDefinition.Body.Instructions.Last().Previous.Operand as VariableReference).Resolve();
            //}

            variable = new VariableDefinition("<>returnValue", this.method.ReturnType.typeReference);
            this.instructions.Variables.Add(variable);
            return variable;
        }

        private bool IsInclosedInHandlers(Instruction instruction)
        {
            foreach (var item in this.method.methodDefinition.Body.ExceptionHandlers)
            {
                if (item.TryStart.Offset >= instruction.Offset && item.TryStart.Offset <= instruction.Offset)
                    return true;

                if (item.HandlerStart != null && item.HandlerStart.Offset >= instruction.Offset && item.HandlerEnd.Offset <= instruction.Offset)
                    return true;
            }

            return false;
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
            var existingVariable = this.instructions.Variables.FirstOrDefault(x => x.Name == name);

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.method.methodDefinition.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff

        #region Comparision stuff

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual Instruction JumpTarget { get; }

        public IIfCode EqualTo(long value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode EqualTo(int value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int32, value).Instructions);

        public IIfCode EqualTo(bool value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Boolean, value).Instructions);

        public IIfCode Is(Type type)
        {
            var jumpTarget = this.JumpTarget == null ? this.processor.Create(OpCodes.Nop) : this.JumpTarget;

            this.instructions.Append(this.processor.Create(OpCodes.Isinst, this.moduleDefinition.Import(type)));
            this.instructions.Append(this.processor.Create(OpCodes.Ldnull));
            this.instructions.Append(this.processor.Create(OpCodes.Cgt_Un));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));

            return new IfCode(this, this.instructions, jumpTarget);
        }

        public IIfCode Is(BuilderType type)
        {
            var jumpTarget = this.JumpTarget == null ? this.processor.Create(OpCodes.Nop) : this.JumpTarget;

            this.instructions.Append(this.processor.Create(OpCodes.Isinst, type.typeReference));
            this.instructions.Append(this.processor.Create(OpCodes.Ldnull));
            this.instructions.Append(this.processor.Create(OpCodes.Cgt_Un));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));

            return new IfCode(this, this.instructions, jumpTarget);
        }

        public IIfCode IsFalse() => this.NotEqualTo(new Instruction[] { this.processor.Create(OpCodes.Ldc_I4_1) });

        public IIfCode IsNotNull() => this.NotEqualTo(new Instruction[] { this.processor.Create(OpCodes.Ldnull) });

        public IIfCode IsNull() => this.EqualTo(new Instruction[] { this.processor.Create(OpCodes.Ldnull) });

        public IIfCode IsTrue() => this.EqualTo(new Instruction[] { this.processor.Create(OpCodes.Ldc_I4_1) });

        public IIfCode NotEqualTo(long value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode NotEqualTo(int value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int32, value).Instructions);

        public IIfCode NotEqualTo(bool value) => this.NotEqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Boolean, value).Instructions);

        private IIfCode EqualTo(IEnumerable<Instruction> instruction)
        {
            var jumpTarget = this.JumpTarget == null ? this.processor.Create(OpCodes.Nop) : this.JumpTarget;

            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        private IIfCode NotEqualTo(IEnumerable<Instruction> instruction)
        {
            var jumpTarget = this.JumpTarget == null ? this.processor.Create(OpCodes.Nop) : this.JumpTarget;

            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brtrue, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        #endregion Comparision stuff
    }
}