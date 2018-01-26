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

        internal InstructionsSet(InstructionsSet instructionsSet, InstructionContainer instructions) : base(instructionsSet.method.OriginType)
        {
            this.method = instructionsSet.method;
            this.processor = instructionsSet.processor;
            this.instructions = instructions;
        }

        protected bool RequiresReturn
        {
            get
            {
                return this.instructions.Count > 0 &&
                    this.instructions.LastOrDefault().OpCode != OpCodes.Rethrow &&
                    this.instructions.LastOrDefault().OpCode != OpCodes.Throw &&
                    this.instructions.LastOrDefault().OpCode != OpCodes.Ret;
            }
        }

        public ICode And()
        {
            this.instructions.Append(this.processor.Create(OpCodes.And));
            return this;
        }

        public ICode And<T>(T[] collection, Func<ICode, T, int, ICode> code)
        {
            if (collection == null || collection.Length == 0)
                return this;

            var coder = this.method.NewCode();
            code(coder, collection[0], 0);

            for (int i = 1; i < collection.Length; i++)
            {
                code(coder, collection[i], i);
                coder.And();
            }

            this.instructions.Append((coder as InstructionsSet).instructions);

            return this;
        }

        public ICode As(BuilderType type)
        {
            var lastInstruction = this.instructions.LastOrDefault();
            if (lastInstruction.IsCallOrNew())
            {
                var lastType = (lastInstruction.Operand as MethodReference)?.ReturnType;

                if (lastType != null && lastType.FullName == type.Fullname)
                    return this;

                if (lastType != null && lastType.IsPrimitive)
                {
                    var paramResult = new ParamResult();
                    paramResult.Type = lastType;

                    this.CastOrBoxValues(this.processor, type.typeReference, paramResult, type.typeDefinition);
                    this.instructions.Append(paramResult.Instructions);
                    return this;
                }
            }
            // Add parameter, field and variable if required later on

            this.instructions.Append(this.processor.Create(OpCodes.Isinst, this.moduleDefinition.ImportReference(type.typeReference)));
            return this;
        }

        public ILocalVariableCode Assign(LocalVariable localVariable) => new LocalVariableInstructionSet(this, localVariable, this.instructions, AssignInstructionType.Store);

        public IFieldCode Assign(Field instance, Field field)
        {
            this.Load(instance);
            return new FieldInstructionsSet(this, field, this.instructions, AssignInstructionType.Store);
        }

        public IFieldCode Assign(LocalVariable instance, Field field)
        {
            this.Load(instance);
            return new FieldInstructionsSet(this, field, this.instructions, AssignInstructionType.Store);
        }

        public IFieldCode Assign(Field field)
        {
            if (!field.IsStatic)
            {
                if (field.OriginType == this.method.AsyncOriginType)
                    this.instructions.Append(processor.Create(OpCodes.Ldarg_0));
            }

            return new FieldInstructionsSet(this, field, this.instructions, AssignInstructionType.Store);
        }

        public IFieldCode Assign(ICode instance, Field field)
        {
            this.instructions.Append((instance as InstructionsSet).instructions);
            return new FieldInstructionsSet(this, field, this.instructions, AssignInstructionType.Store);
        }

        public IFieldCode AssignToField(string fieldName)
        {
            if (!this.method.AsyncOriginType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.AsyncOriginType}'");

            var field = this.method.AsyncOriginType.Fields[fieldName];
            return this.Assign(field);
        }

        public ILocalVariableCode AssignToLocalVariable(int localVariableIndex) => this.Assign(new LocalVariable(this.method.OriginType, this.method.methodDefinition.Body.Variables[localVariableIndex]));

        public ILocalVariableCode AssignToLocalVariable(string localVariableName)
        {
            var variable = this.method.GetLocalVariable(localVariableName);

            if (variable == null)
                throw new KeyNotFoundException($"The local variable with the name '{localVariableName}' does not exist in '{method.OriginType}'");

            return this.Assign(new LocalVariable(this.method.OriginType, variable, localVariableName));
        }

        public ICode Call(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Call, parameters);

        public ICode Call(Crumb instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Call(Field instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Call(LocalVariable instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Call(ICode instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Call, parameters);

        public ICode Callvirt(Method method, params object[] parameters) => CallInternal(null, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(Crumb instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(Field instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(LocalVariable instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Callvirt(ICode instance, Method method, params object[] parameters) => CallInternal(instance, method, OpCodes.Callvirt, parameters);

        public ICode Context(Action<ICode> body)
        {
            body(this);
            return this;
        }

        public Method Copy(Modifiers modifiers, string newName)
        {
            var method = this.method.OriginType.typeDefinition.Methods.Get(newName);

            if (method != null)
                return new Method(this.method.OriginType, method);

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
                method.Body.Variables.Add(new VariableDefinition(item.VariableType));

            method.Body.InitLocals = this.method.methodDefinition.Body.InitLocals;

            this.method.OriginType.typeDefinition.Methods.Add(method);
            CopyMethod(method);

            method.Body.OptimizeMacros();

            return new Method(this.method.OriginType, method);
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

        public Position GetFirstOrDefaultPosition(Func<Instruction, bool> predicate)
        {
            if (this.method.methodDefinition.Body == null || this.method.methodDefinition.Body.Instructions == null)
                return null;

            foreach (var item in this.method.methodDefinition.Body.Instructions)
                if (predicate(item))
                    return new Position(this.method, item);

            return null;
        }

        public Crumb GetParametersArray()
        {
            var variableName = "<>params_" + this.method.Identification;
            if (this.method.GetLocalVariable(variableName) == null)
            {
                var objectArrayType = this.method.OriginType.Builder.GetType(typeof(object[]));
                var variable = this.CreateVariable(variableName, objectArrayType);
                var newInstructions = new List<Instruction>
                {
                    processor.Create(OpCodes.Ldc_I4, this.method.methodReference.Parameters.Count),
                    processor.Create(OpCodes.Newarr, (objectArrayType.typeReference as ArrayType).ElementType),
                    processor.Create(OpCodes.Stloc, variable.variable)
                };

                foreach (var parameter in this.method.methodReference.Parameters)
                    newInstructions.AddRange(IlHelper.ProcessParam(parameter, variable.variable));
                // Insert the call in the beginning of the instruction list
                this.instructions.Insert(0, newInstructions);
            }

            return new Crumb { CrumbType = CrumbTypes.Parameters, Name = variableName };
        }

        public LocalVariable GetReturnVariable() => new LocalVariable(this.method.type, this.GetOrCreateReturnVariable(), "<>returnValue");

        public void Insert(InsertionAction action, Position position)
        {
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
            if (processor.Body == null || processor.Body.Instructions.Count == 0)
                processor.Append(processor.Create(OpCodes.Ret));

            if (position == InsertionPosition.CtorBeforeInit)
            {
                instructionPosition = processor.Body.Instructions[0];
            }
            else if (position == InsertionPosition.Beginning)
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
                        item.Operand = this.instructions.FirstOrDefault();
                }
            }

            processor.InsertBefore(instructionPosition, this.instructions);

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.ReplaceReturns();

            this.CleanLocalVariableList();
            this.method.methodDefinition.Body.InitLocals = this.method.methodDefinition.Body.Variables.Count > 0;
            this.method.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
        }

        public ICode Jump(Position position)
        {
            this.instructions.Append(this.processor.Create(OpCodes.Br, position.instruction));
            return this;
        }

        public ICode Leave(Position position)
        {
            this.instructions.Append(this.processor.Create(OpCodes.Leave, position.instruction));
            return this;
        }

        public IFieldCode Load(Field instance, Field field)
        {
            this.Load(instance);
            this.instructions.Append(processor.Create(OpCodes.Ldfld, field.fieldRef));
            return this.CreateFieldInstructionSet(field, AssignInstructionType.Load);
        }

        public IFieldCode Load(LocalVariable instance, Field field)
        {
            this.Load(instance);
            this.instructions.Append(processor.Create(OpCodes.Ldfld, field.fieldRef));
            return this.CreateFieldInstructionSet(field, AssignInstructionType.Load);
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
            if (localVariable.variable.VariableType.IsValueType && !localVariable.variable.VariableType.IsPrimitive)
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
            if (!this.method.OriginType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.OriginType}'");

            var field = this.method.OriginType.Fields[fieldName];
            return this.Load(field);
        }

        public ILocalVariableCode LoadVariable(string variableName)
        {
            var variable = this.method.GetLocalVariable(variableName);

            if (variable == null)
                throw new KeyNotFoundException($"The local variable with the name '{variableName}' does not exist in '{method.OriginType}'");

            return this.Load(new LocalVariable(this.method.OriginType, variable, variableName));
        }

        public ILocalVariableCode LoadVariable(int variableIndex)
        {
            var localvariable = this.method.methodDefinition.Body.Variables[variableIndex];
            return this.Load(new LocalVariable(this.method.OriginType, localvariable));
        }

        public ICode Newarr(BuilderType type, int size)
        {
            this.instructions.Append(this.AddParameter(this.processor, null, size).Instructions);
            this.instructions.Append(this.processor.Create(OpCodes.Newarr, this.moduleDefinition.ImportReference(type.typeReference)));

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
                //if (constructor.methodDefinition.Parameters.Count != parameters.Length)
                //    this.LogWarning($"Parameter count of constructor {constructor.Name} does not match with the passed parameters. Expected: {constructor.methodDefinition.Parameters.Count}, is: {parameters.Length}");

                var startParam = constructor.type.IsDelegate ? 1 : 0;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var inst = this.AddParameter(this.processor, constructor.methodDefinition.Parameters[i + startParam].ParameterType, parameters[i]);
                    this.instructions.Append(inst.Instructions);
                }
            }

            //var ctor = constructor.methodReference;

            //if(ctor.ContainsGenericParameter && !ctor.IsGenericInstance)
            //    ctor = ctor.MakeHostInstanceGeneric(ctor)

            //foreach (var item in constructor.methodReference)
            //    this.LogInfo(item);

            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.ImportReference(constructor.methodReference)));
            this.StoreCall();
            return this;
        }

        public ICode NewObj(AttributedProperty attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedType attribute) => this.NewObj(attribute.customAttribute);

        public ICode Or()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Or));
            return this;
        }

        public ICode Or<T>(T[] collection, Func<ICode, T, int, ICode> code)
        {
            if (collection == null || collection.Length == 0)
                return this;

            var coder = this.method.NewCode();
            code(coder, collection[0], 0);

            for (int i = 1; i < collection.Length; i++)
            {
                code(coder, collection[i], i);
                coder.Or();
            }

            this.instructions.Append((coder as InstructionsSet).instructions);

            return this;
        }

        public ICode OriginalBody()
        {
            var instructions = CopyMethodBody(this.method.methodDefinition, this.method.methodDefinition.Body.Variables);

            // special case for .ctor
            if (this.method.IsCtor)
            {
                var newBody = instructions.Instructions.ToList();

                // remove everything until base call
                var first = newBody.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                if (first == null)
                    throw new NullReferenceException($"The constructor of type '{this.method.OriginType}' seems to have no call to base class.");

                var firstIndex = newBody.IndexOf(first);
                newBody = newBody.GetRange(firstIndex + 1, newBody.Count - firstIndex - 1);

                instructions = (newBody, instructions.Exceptions);
            }

            if (this.method.methodDefinition.ReturnType.FullName == "System.Void")
            {
                // On void method we just simply remove the return and replace all other jumps to the
                // ret instruction with a ret instruction
                var returnInstruction = instructions.Instructions.Last();

                foreach (var item in instructions.Instructions)
                {
                    if (item.Operand == returnInstruction)
                    {
                        item.Operand = null;
                        item.OpCode = OpCodes.Ret;
                    }
                }
            }

            this.instructions.Append(instructions.Instructions);
            this.instructions.ExceptionHandlers.AddRange(instructions.Exceptions);

            return this;
        }

        public ICode OriginalBodyNewMethod()
        {
            var newMethod = this.Copy(Modifiers.Private, $"<{this.method.Name}>m__original");

            for (int i = 0; i < this.method.Parameters.Length + (this.method.IsStatic ? 0 : 1); i++)
                this.instructions.Append(processor.Create(OpCodes.Ldarg, i));

            this.instructions.Append(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(newMethod.methodReference)));

            return this;
        }

        public ICode Pop()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Pop));
            return this;
        }

        public (Position Beginning, Position End) Replace(Position position)
        {
            if (position.instruction == null)
                throw new ArgumentNullException(nameof(position.instruction));

            var index = this.method.methodDefinition.Body.Instructions.IndexOf(position.instruction);
            processor.Remove(position.instruction);
            processor.InsertBefore(index, this.instructions);

            return (new Position(this.method, this.instructions[0]), new Position(this.method, this.instructions.LastOrDefault()));
        }

        public void Replace()
        {
            // Special case for .ctors
            if (this.method.IsCtor && this.method.methodDefinition.Body?.Instructions != null && this.method.methodDefinition.Body.Instructions.Count > 0)
            {
                var first = this.method.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                if (first == null)
                    throw new NullReferenceException($"The constructor of type '{this.method.OriginType}' seems to have no call to base class.");

                // In ctors we only replace the instructions after base call
                var callsBeforeBase = this.method.methodDefinition.Body.Instructions.TakeWhile(x => x != first).ToList();
                callsBeforeBase.Add(first);

                this.method.methodDefinition.Body.Instructions.Clear();
                this.method.methodDefinition.Body.ExceptionHandlers.Clear();

                processor.Append(callsBeforeBase);
                processor.Append(this.instructions);
            }
            else
            {
                this.method.methodDefinition.Body.Instructions.Clear();
                this.method.methodDefinition.Body.ExceptionHandlers.Clear();

                processor.Append(this.instructions);
            }

            foreach (var item in this.instructions.ExceptionHandlers)
                processor.Body.ExceptionHandlers.Add(item);

            this.ReplaceReturns();

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

        public ICode ReturnDefault()
        {
            if (!this.method.IsVoid)
            {
                var variable = this.GetOrCreateReturnVariable();
                var defaultValue = this.method.ReturnType.DefaultValue;
                var inst = this.AddParameter(this.processor,
                    this.method.ReturnType.GenericArguments().Any() ?
                        this.method.ReturnType.GetGenericArgument(0).typeReference :
                        this.method.ReturnType.typeReference, defaultValue);

                this.instructions.Append(inst.Instructions);
            }

            this.Return();

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
            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(exception).GetMethodReference(".ctor", new Type[] { typeof(string) }))));
            this.instructions.Append(processor.Create(OpCodes.Throw));
            return this;
        }

        public ITry Try(Action<ITryCode> body)
        {
            if (this.instructions.Count == 0)
                this.instructions.Append(processor.Create(OpCodes.Nop));

            var markerStart = this.instructions.LastOrDefault();
            body(this);

            if (this.RequiresReturn)
                this.instructions.Append(this.processor.Create(OpCodes.Ret));

            return new MarkerInstructionSet(this, MarkerType.Try, markerStart ?? this.instructions.FirstOrDefault(), this.instructions);
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

            if (type.IsEnum)
            {
                this.moduleDefinition.ImportReference(type.GetType());
                type = Enum.GetUnderlyingType(type);
                parameter = Convert.ChangeType(parameter, type);
            }

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
                            result.Type = this.moduleDefinition.ImportReference(this.method.methodDefinition.Parameters[crumb.Index.Value].ParameterType);
                        }
                        else
                        {
                            var variable = char.IsNumber(crumb.Name, 0) ? this.method.methodDefinition.Body.Variables[int.Parse(crumb.Name)] : this.method.GetLocalVariable(crumb.Name);

                            result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                            result.Type = this.moduleDefinition.ImportReference(variable.VariableType);
                        }
                        break;

                    case CrumbTypes.This:
                        result.Instructions.Add(processor.Create(this.method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                        result.Type = this.method.OriginType.typeReference;
                        break;

                    case CrumbTypes.InitObj:
                        {
                            var variable = this.CreateVariable(crumb.TypeReference);
                            result.Instructions.Add(processor.Create(OpCodes.Ldloca, variable.variable));
                            result.Instructions.Add(processor.Create(OpCodes.Initobj, crumb.TypeReference));
                            result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable.variable));
                            result.Type = crumb.TypeReference;
                            break;
                        }
                    case CrumbTypes.DefaultTask:
                        {
                            var taskType = this.method.type.Builder.GetType("System.Threading.Tasks.Task");
                            var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(typeof(int));
                            var code = this.NewCode().Call(resultFrom, 0);

                            result.Instructions.AddRange((code as InstructionsSet).instructions);
                            result.Type = this.method.ReturnType.typeReference;
                            break;
                        }

                    case CrumbTypes.DefaultTaskOfT:
                        {
                            var returnType = this.method.ReturnType.GetGenericArgument(0);
                            var taskType = this.method.type.Builder.GetType("System.Threading.Tasks.Task");
                            var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(returnType);
                            var code = this.NewCode().Call(resultFrom, returnType.DefaultValue);

                            result.Instructions.AddRange((code as InstructionsSet).instructions);
                            result.Type = returnType.typeReference;
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            else if (type == typeof(TypeReference) || type == typeof(TypeDefinition))
            {
                var bt = parameter as TypeReference;
                result.Instructions.AddRange(this.TypeOf(processor, bt));
                result.Type = this.moduleDefinition.ImportReference(typeof(Type));
            }
            else if (type == typeof(BuilderType))
            {
                var bt = parameter as BuilderType;
                result.Instructions.AddRange(this.TypeOf(processor, bt.typeReference));
                result.Type = this.moduleDefinition.ImportReference(typeof(Type));
            }
            else if (type == typeof(Method))
            {
                var method = parameter as Method;

                if (targetType.FullName == typeof(IntPtr).FullName)
                {
                    if (!method.IsStatic && method.OriginType != this.method.OriginType && this.method.OriginType.IsAsyncStateMachine)
                    {
                        var instance = this.method.AsyncMethodHelper.Instance;
                        var inst = this.AddParameter(processor, targetType, instance);
                        result.Instructions.AddRange(inst.Instructions);
                    }
                    else
                        result.Instructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));

                    result.Instructions.Add(processor.Create(OpCodes.Ldftn, method.methodReference));
                    result.Type = this.moduleDefinition.TypeSystem.IntPtr;
                }
                else
                {
                    var methodBaseRef = this.moduleDefinition.ImportReference(typeof(System.Reflection.MethodBase));
                    // methodof
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.methodReference));
                    result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.OriginType.typeReference));
                    result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(methodBaseRef.BetterResolve().Methods.FirstOrDefault(x => x.Name == "GetMethodFromHandle" && x.Parameters.Count == 2))));

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
            else if (parameter is Position)
            {
                var instruction = (parameter as Position).instruction;
                result.Instructions.Add(instruction);
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
            if (targetDef == null && result.Type.Resolve() != null) /* This happens if the target type is a generic */
                result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            else if ((targetDef.FullName == typeof(string).FullName || result.Type.FullName == typeof(object).FullName || targetDef.IsInterface) && !targetType.IsValueType && !targetType.IsArray && !targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                result.Instructions.Add(processor.Create(OpCodes.Isinst, this.moduleDefinition.ImportReference(targetType)));
            else if (targetDef.IsEnum)
            {
                if (result.Type.FullName == typeof(string).FullName)
                {
                    result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                    result.Instructions.AddRange(this.TypeOf(processor, this.moduleDefinition.ImportReference(targetType)));
                    result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
                }
                else
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));

                // Bug #23
                //result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                //result.Instructions.AddRange(this.TypeOf(processor, this.moduleDefinition.ImportReference(targetType)));
                //result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
            }
            else if (result.Type.FullName == typeof(object).FullName && (targetType.IsArray || targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1")))
            {
                var childType = this.moduleDefinition.GetChildrenType(targetType);
                var castMethod = this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(System.Linq.Enumerable)).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(null, childType));
                var toArrayMethod = this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(System.Linq.Enumerable)).GetMethodReference("ToArray", 1).MakeGeneric(null, childType));

                result.Instructions.Add(processor.Create(OpCodes.Isinst, this.moduleDefinition.ImportReference(typeof(IEnumerable))));
                result.Instructions.Add(processor.Create(OpCodes.Call, castMethod));

                if (targetType.IsArray)
                    result.Instructions.Add(processor.Create(OpCodes.Call, toArrayMethod));
            }
            else if (result.Type.FullName == typeof(object).FullName && targetDef.IsValueType)
            {
                if (targetDef.FullName == typeof(int).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(uint).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToUInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(bool).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToBoolean", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(byte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(char).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToChar", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(DateTime).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToDateTime", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(decimal).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToDecimal", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(double).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToDouble", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(short).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(long).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToInt64", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(sbyte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToSByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(float).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToSingle", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ushort).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToUInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ulong).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, this.moduleDefinition.ImportReference(this.moduleDefinition.ImportReference(typeof(Convert)).GetMethodReference("ToUInt64", new Type[] { typeof(object) }))));
                else result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            }
            else if ((result.Type.Resolve() == null || result.Type.IsValueType) && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName == "System.Object")
            {
                // Nope nothing....
            }
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName != result.Type.FullName && this.AreReferenceAssignable(targetType, this.moduleDefinition.ImportReference(result.Type)))
                result.Instructions.Add(processor.Create(OpCodes.Castclass, this.moduleDefinition.ImportReference(result.Type)));
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
                result.Add(processor.Create(OpCodes.Newarr, this.moduleDefinition.ImportReference(attributeArgument.Type.GetElementType())));

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
            /*
             Does not do anything now.... Removed because... Buggy

             */

            //var usedVariables = new List<VariableDefinition>();
            //var instructions = this.method.methodDefinition.Body.Instructions;
            //var variables = this.method.methodDefinition.Body.Variables;

            //for (int i = 0; i < instructions.Count; i++)
            //{
            //    VariableDefinition variable = null;
            //    if (instructions[i].OpCode == OpCodes.Ldloc_0)
            //    {
            //        variable = variables[0];
            //        instructions[i].OpCode = OpCodes.Ldloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Ldloc_1)
            //    {
            //        variable = variables[1];
            //        instructions[i].OpCode = OpCodes.Ldloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Ldloc_2)
            //    {
            //        variable = variables[2];
            //        instructions[i].OpCode = OpCodes.Ldloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Ldloc_3)
            //    {
            //        variable = variables[3];
            //        instructions[i].OpCode = OpCodes.Ldloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Stloc_0)
            //    {
            //        variable = variables[0];
            //        instructions[i].OpCode = OpCodes.Stloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Stloc_1)
            //    {
            //        variable = variables[1];
            //        instructions[i].OpCode = OpCodes.Stloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Stloc_2)
            //    {
            //        variable = variables[2];
            //        instructions[i].OpCode = OpCodes.Stloc;
            //    }
            //    else if (instructions[i].OpCode == OpCodes.Stloc_3)
            //    {
            //        variable = variables[3];
            //        instructions[i].OpCode = OpCodes.Stloc;
            //    }
            //    else if (
            //        instructions[i].OpCode == OpCodes.Ldloca_S ||
            //        instructions[i].OpCode == OpCodes.Ldloc_S)
            //    {
            //        variable = variables[(int)instructions[i].Operand];
            //        instructions[i].OpCode = OpCodes.Ldloc;
            //    }
            //    else if (
            //        instructions[i].OpCode == OpCodes.Stloc_S)
            //    {
            //        variable = variables[(int)instructions[i].Operand];
            //        instructions[i].OpCode = OpCodes.Stloc;
            //    }
            //    else if (
            //        instructions[i].OpCode == OpCodes.Ldloc ||
            //        instructions[i].OpCode == OpCodes.Ldloca ||
            //        instructions[i].OpCode == OpCodes.Stloc)
            //        variable = instructions[i].Operand as VariableDefinition;

            //    if (variable != null)
            //    {
            //        instructions[i].Operand = variable;
            //        usedVariables.Add(variable);
            //    }
            //}

            //foreach (var item in variables.Where(x => !usedVariables.Contains(x)).ToArray())
            //    variables.Remove(item);
        }

        protected virtual IFieldCode CreateFieldInstructionSet(Field field, AssignInstructionType instructionType) => new FieldInstructionsSet(this, field, this.instructions, instructionType);

        protected virtual ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable, AssignInstructionType instructionType) => new LocalVariableInstructionSet(this, localVariable, this.instructions, instructionType);

        protected void InstructionDebug() => this.Log(this.instructions);

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

            if (this.method.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            if (this.method.IsVoid || this.instructions.LastOrDefault().OpCode != OpCodes.Ret)
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
                var realReturn = this.method.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsValueOpCode() && realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    resultJump = true;
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    this.processor.InsertBefore(realReturn, this.AddParameter(this.processor, this.method.ReturnType.typeReference, this.GetOrCreateReturnVariable()).Instructions);

                    realReturn = realReturn.Previous;
                }
                else if (realReturn.Previous.IsLoadField() || realReturn.Previous.IsLoadLocal() || realReturn.Previous.OpCode == OpCodes.Ldnull)
                {
                    realReturn = realReturn.Previous;

                    // Think twice before removing this ;)
                    if (realReturn.OpCode == OpCodes.Ldfld || realReturn.OpCode == OpCodes.Ldflda)
                        realReturn = realReturn.Previous;
                }
                else
                    realReturn = realReturn.Previous;

                for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = this.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    if (this.IsInclosedInHandlers(instruction))
                    {
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = realReturn;

                        if (this.method.methodDefinition.ReturnType.FullName == "System.Void" || this.method.methodDefinition.ReturnType.FullName == "System.Threading.Task")
                            continue;

                        if (resultJump)
                        {
                            var returnVariable = this.GetOrCreateReturnVariable();
                            var previousInstruction = instruction.Previous;

                            if (previousInstruction != null && previousInstruction.IsLoadLocal())
                            {
                                if (
                                    (returnVariable.Index == 0 && previousInstruction.OpCode == OpCodes.Ldloc_0) ||
                                    (returnVariable.Index == 1 && previousInstruction.OpCode == OpCodes.Ldloc_1) ||
                                    (returnVariable.Index == 2 && previousInstruction.OpCode == OpCodes.Ldloc_2) ||
                                    (returnVariable.Index == 3 && previousInstruction.OpCode == OpCodes.Ldloc_3) ||
                                    (previousInstruction.OpCode == OpCodes.Ldloc_S && returnVariable.Index == (int)previousInstruction.Operand) ||
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
                                    )
                                {
                                    this.ReplaceJumps(previousInstruction, instruction);

                                    // In this case also remove the redundant ldloc opcode
                                    i--;
                                    this.method.methodDefinition.Body.Instructions.Remove(previousInstruction);
                                    continue;
                                }
                            }

                            if (previousInstruction != null && previousInstruction.IsStoreLocal())
                            {
                                if (
                                    (returnVariable.Index == 0 && previousInstruction.OpCode == OpCodes.Stloc_0) ||
                                    (returnVariable.Index == 1 && previousInstruction.OpCode == OpCodes.Stloc_1) ||
                                    (returnVariable.Index == 2 && previousInstruction.OpCode == OpCodes.Stloc_2) ||
                                    (returnVariable.Index == 3 && previousInstruction.OpCode == OpCodes.Stloc_3) ||
                                    (previousInstruction.OpCode == OpCodes.Stloc_S && returnVariable.Index == (int)previousInstruction.Operand) ||
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
                                    )
                                    continue; // Just continue and do not add an additional store opcode
                            }

                            this.processor.InsertBefore(instruction, this.processor.Create(OpCodes.Stloc, returnVariable));
                        }
                    }
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

        private static void SetCorrectJumpPoints(List<(int CurrentIndex, int Index)> jumps, IList<Instruction> methodInstructions)
        {
            foreach (var j in jumps.GroupBy(x => x.CurrentIndex))
            {
                if (methodInstructions[j.Key].OpCode == OpCodes.Switch)
                {
                    var instructions = new List<Instruction>();

                    foreach (var switches in j)
                        instructions.Add(methodInstructions[switches.Index]);

                    methodInstructions[j.Key].Operand = instructions.ToArray();
                }
                else
                    methodInstructions[j.Key].Operand = methodInstructions[j.First().Index];
            }
        }

        private ICode CallInternal(object instance, Method method, OpCode opcode, params object[] parameters)
        {
            if (instance != null)
                this.instructions.Append(this.AddParameter(this.processor, null, instance).Instructions);

            if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb crumb && crumb.UnPackArray)
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
            else if (parameters != null && parameters.Length > 0 && parameters[0] is Crumb crumb2 && crumb2.Index < 0)
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli)
                    opcode = OpCodes.Callvirt;

                for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    var inst = this.AddParameter(this.processor, this.moduleDefinition.ImportReference(parameterType), Crumb.GetParameter(i));
                    this.instructions.Append(inst.Instructions);
                }
            }
            else
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli)
                    opcode = OpCodes.Callvirt;

                if (parameters != null)
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                            method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                            method.methodDefinition.Parameters[i].ParameterType;

                        var inst = this.AddParameter(this.processor, this.moduleDefinition.ImportReference(parameterType), parameters[i]);
                        this.instructions.Append(inst.Instructions);
                    }
            }

            try
            {
                this.instructions.Append(processor.Create(opcode, this.moduleDefinition.ImportReference(method.methodReference)));
            }
            catch (NullReferenceException)
            {
                this.instructions.Append(processor.Create(opcode, this.moduleDefinition.ImportReference(method.methodReference, this.method.methodReference)));
            }

            if (!method.IsVoid)
                this.StoreCall();

            return this;
        }

        private ExceptionHandler Copy(ExceptionHandler original, IList<Instruction> instructionList) => new ExceptionHandler(original.HandlerType)
        {
            CatchType = original.CatchType,
            FilterStart = GetInstruction(original.FilterStart, instructionList),
            HandlerEnd = GetInstruction(original.HandlerEnd, instructionList),
            HandlerStart = GetInstruction(original.HandlerStart, instructionList),
            TryEnd = GetInstruction(original.TryEnd, instructionList),
            TryStart = GetInstruction(original.TryStart, instructionList)
        };

        private void CopyMethod(MethodDefinition method)
        {
            var methodProcessor = method.Body.GetILProcessor();
            var instructions = CopyMethodBody(this.method.methodDefinition, this.method.methodDefinition.Body.Variables);
            methodProcessor.Append(instructions.Instructions);

            foreach (var item in instructions.Exceptions)
                method.Body.ExceptionHandlers.Add(item);
        }

        private (IEnumerable<Instruction> Instructions, IEnumerable<ExceptionHandler> Exceptions) CopyMethodBody(MethodDefinition originalMethod, IList<VariableDefinition> variableDefinition)
        {
            if (this.method.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            var methodProcessor = originalMethod.Body.GetILProcessor();
            var resultingInstructions = new List<(Instruction Original, Instruction Target)>();
            var exceptionList = new List<ExceptionHandler>();
            var jumps = new List<(int CurrentIndex, int Index)>();

            for (int i = 0; i < (originalMethod.Body?.Instructions.Count ?? 0); i++)
            {
                var item = originalMethod.Body.Instructions[i];

                if (item.Operand is Instruction)
                    resultingInstructions.Add((item, CreateInstruction(originalMethod.Body.Instructions, item, i, methodProcessor, ref jumps)));
                else if (item.Operand is Instruction[])
                {
                    var instructions = item.Operand as Instruction[];
                    var newInstructions = new Instruction[instructions.Length];
                    for (int x = 0; x < instructions.Length; x++)
                        newInstructions[x] = CreateInstruction(originalMethod.Body.Instructions, instructions[x], i, methodProcessor, ref jumps);

                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = newInstructions;

                    resultingInstructions.Add((item, instruction));
                }
                //else if (item.Operand is CallSite )
                //    throw new NotImplementedException($"Unknown operand '{item.OpCode.ToString()}' '{item.Operand.GetType().FullName}'");
                else
                {
                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = item.Operand;
                    resultingInstructions.Add((item, instruction));

                    // Set the correct variable def if required
                    if (instruction.Operand is VariableDefinition variable)
                        instruction.Operand = variableDefinition[variable.Index];
                }
            }

            SetCorrectJumpPoints(jumps, resultingInstructions.Select(x => x.Target).ToList());
            Instruction getInstruction(Instruction instruction) => instruction == null ? null : resultingInstructions.FirstOrDefault(x => x.Original.Offset == instruction.Offset).Target ?? null;

            ExceptionHandler copyHandler(ExceptionHandler original) => new ExceptionHandler(original.HandlerType)
            {
                CatchType = original.CatchType,
                FilterStart = getInstruction(original.FilterStart),
                HandlerEnd = getInstruction(original.HandlerEnd),
                HandlerStart = getInstruction(original.HandlerStart),
                TryEnd = getInstruction(original.TryEnd),
                TryStart = getInstruction(original.TryStart)
            };

            foreach (var item in originalMethod.Body.ExceptionHandlers)
                exceptionList.Add(copyHandler(item));

            return (resultingInstructions.Select(x => x.Target), exceptionList);
        }

        private Instruction CreateInstruction(IList<Instruction> instructions, Instruction instructionTarget, int currentIndex, ILProcessor processor, ref List<(int, int)> jumps)
        {
            GetJumpPoints(instructions, instructionTarget, currentIndex, ref jumps);

            var instructionResult = processor.Create(OpCodes.Nop);
            instructionResult.OpCode = instructionTarget.OpCode;
            return instructionResult;
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
                   this.moduleDefinition.ImportReference(Enum.GetUnderlyingType(type)) : this.moduleDefinition.ImportReference(this.GetTypeDefinition(type))));

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
                return ctorCall.Next;

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
            })?.Next;
        }

        private Instruction GetInstruction(Instruction instruction, IList<Instruction> instructionList)
        {
            if (instruction == null)
                return null;

            var index = this.method.methodDefinition.Body.Instructions.IndexOf(instruction.Offset);
            return instructionList[index];
        }

        private void GetJumpPoints(IList<Instruction> instructions, Instruction instructionTarget, int currentIndex, ref List<(int, int)> jumps)
        {
            var operand = instructionTarget.Operand as Instruction;

            if (operand == null)
                return;

            var index = instructions.IndexOf(operand);

            if (index >= 0)
                jumps.Add((currentIndex, index));
            else
            {
                index = instructions.IndexOf(operand.Offset);

                if (index >= 0)
                    jumps.Add((currentIndex, index));
            }
        }

        private VariableDefinition GetOrCreateReturnVariable()
        {
            var variable = this.method.GetLocalVariable("<>returnValue");

            if (variable != null)
                return variable;

            if (this.method.methodDefinition.Body.Instructions.Count > 1)
            {
                var lastOpCode = this.method.methodDefinition.Body.Instructions.Last().Previous;

                if (lastOpCode.IsLoadLocal())
                {
                    if (lastOpCode.Operand is int index && this.method.methodDefinition.Body.Variables.Count > index)
                        variable = this.method.methodDefinition.Body.Variables[index];

                    if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                        variable = variableReference;

                    if (variable == null)
                        if (lastOpCode.OpCode == OpCodes.Ldloc_0) variable = this.method.methodDefinition.Body.Variables[0];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_1) variable = this.method.methodDefinition.Body.Variables[1];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_2) variable = this.method.methodDefinition.Body.Variables[2];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_3) variable = this.method.methodDefinition.Body.Variables[3];

                    if (variable != null)
                    {
                        this.method.AddLocalVariable("<>returnValue", variable);
                        return variable;
                    }
                }
            }

            return this.method.AddLocalVariable("<>returnValue", new VariableDefinition(this.method.ReturnType.typeReference));
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

        private void ReplaceJumps(Instruction tobeReplaced, Instruction replacement)
        {
            for (var i = 0; i < this.method.methodDefinition.Body.Instructions.Count - 1; i++)
            {
                var instruction = this.method.methodDefinition.Body.Instructions[i];

                if (instruction.Operand == tobeReplaced)
                    instruction.Operand = replacement;
            }

            for (var i = 0; i < this.method.methodDefinition.Body.ExceptionHandlers.Count; i++)
            {
                var handler = this.method.methodDefinition.Body.ExceptionHandlers[i];

                if (handler.FilterStart == tobeReplaced)
                    handler.FilterStart = replacement;

                if (handler.HandlerEnd == tobeReplaced)
                    handler.HandlerEnd = replacement;

                if (handler.HandlerStart == tobeReplaced)
                    handler.HandlerStart = replacement;

                if (handler.TryEnd == tobeReplaced)
                    handler.TryEnd = replacement;

                if (handler.TryStart == tobeReplaced)
                    handler.TryStart = replacement;
            }
        }

        #region Variable

        public LocalVariable CreateVariable(string name, Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(name, method.OriginType);

            return this.CreateVariable(name, method.ReturnType);
        }

        public LocalVariable CreateVariable(string name, BuilderType type)
        {
            var existingVariable = this.method.GetLocalVariable(name);

            if (existingVariable != null)
                return new LocalVariable(this.method.type, existingVariable, name);

            var newVariable = new VariableDefinition(this.moduleDefinition.ImportReference(type.typeReference));
            this.method.AddLocalVariable(name, newVariable);

            return new LocalVariable(this.method.type, newVariable, name);
        }

        public LocalVariable CreateVariable(Type type)
        {
            var newVariable = new VariableDefinition(this.moduleDefinition.ImportReference(GetTypeDefinition(type)));
            var name = "<>var_" + CecilatorBase.GenerateName();
            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
        }

        public LocalVariable CreateVariable(Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(method.OriginType);

            return this.CreateVariable(method.ReturnType);
        }

        public LocalVariable CreateVariable(TypeReference type)
        {
            var newVariable = new VariableDefinition(this.moduleDefinition.ImportReference(type));
            var name = "<>var_" + CecilatorBase.GenerateName();
            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
        }

        public LocalVariable CreateVariable(BuilderType type)
        {
            var newVariable = new VariableDefinition(this.moduleDefinition.ImportReference(type.typeReference));
            var name = "<>var_" + CecilatorBase.GenerateName();
            this.method.AddLocalVariable(name, newVariable);
            return new LocalVariable(this.method.type, newVariable, name);
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

        public IIfCode EqualTo(string value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode EqualTo(long value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int64, value).Instructions);

        public IIfCode EqualTo(int value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Int32, value).Instructions);

        public IIfCode EqualTo(bool value) => this.EqualTo(this.AddParameter(this.processor, this.moduleDefinition.TypeSystem.Boolean, value).Instructions);

        public IIfCode Is(Type type)
        {
            var jumpTarget = this.JumpTarget ?? this.processor.Create(OpCodes.Nop);

            this.instructions.Append(this.processor.Create(OpCodes.Isinst, this.moduleDefinition.ImportReference(type)));
            this.instructions.Append(this.processor.Create(OpCodes.Ldnull));
            this.instructions.Append(this.processor.Create(OpCodes.Cgt_Un));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));

            return new IfCode(this, this.instructions, jumpTarget);
        }

        public IIfCode Is(BuilderType type)
        {
            var jumpTarget = this.JumpTarget ?? this.processor.Create(OpCodes.Nop);

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
            var jumpTarget = this.JumpTarget ?? this.processor.Create(OpCodes.Nop);

            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brfalse, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        private IIfCode NotEqualTo(IEnumerable<Instruction> instruction)
        {
            var jumpTarget = this.JumpTarget ?? this.processor.Create(OpCodes.Nop);

            this.instructions.Append(instruction);
            this.instructions.Append(this.processor.Create(OpCodes.Ceq));
            this.instructions.Append(this.processor.Create(OpCodes.Brtrue, jumpTarget));
            return new IfCode(this, this.instructions, jumpTarget);
        }

        #endregion Comparision stuff
    }
}