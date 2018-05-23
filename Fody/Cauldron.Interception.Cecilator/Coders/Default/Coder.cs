using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public class Coder :
        CoderBase<Coder, Coder>,
        ICallMethod<CallCoder>,
        IExitOperators,
        IFieldOperationsExtended<FieldCoder>,
        IVariableOperationsExtended<VariableCoder>,
        IArgOperationsExtended<ArgCoder>,
        ICasting<Coder>,
        ILoadValue<Coder>,
        INewObj<CallCoder>
    {
        internal Coder(Method method) : base(method)
        {
        }

        internal Coder(InstructionBlock instructionBlock) : base(instructionBlock)
        {
        }

        public override Coder End => new Coder(this);

        public static implicit operator InstructionBlock(Coder coder) => coder.instructions;

        public Coder Context(Func<Coder, Coder> code)
        {
            this.instructions.Append(code(this.NewCoder()));
            return this;
        }

        public Method Copy(Modifiers modifiers, string newName)
        {
            var method = this.instructions.associatedMethod.OriginType.GetMethod(newName, false, this.AssociatedMethod.Parameters);

            if (method != null)
                return method;

            var attributes = modifiers.ToMethodAttributes();
            var methodDefinition = new MethodDefinition(newName, attributes, this.instructions.associatedMethod.methodReference.ReturnType);

            foreach (var item in this.instructions.associatedMethod.methodReference.Parameters)
                methodDefinition.Parameters.Add(item);

            foreach (var item in this.instructions.associatedMethod.methodReference.GenericParameters)
                methodDefinition.GenericParameters.Add(item);

            foreach (var item in this.instructions.associatedMethod.methodDefinition.Body.Variables)
                methodDefinition.Body.Variables.Add(new VariableDefinition(item.VariableType));

            methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.InitLocals;

            this.instructions.associatedMethod.OriginType.typeDefinition.Methods.Add(methodDefinition);
            CopyMethod(methodDefinition);

            methodDefinition.Body.OptimizeMacros();

            return new Method(this.instructions.associatedMethod.OriginType, methodDefinition);
        }

        public Coder DefaultValue()
        {
            if (!this.instructions.associatedMethod.IsVoid)
            {
                var variable = this.GetOrCreateReturnVariable();
                var defaultValue = this.instructions.associatedMethod.ReturnType.DefaultValue;

                this.instructions.Append(InstructionBlock.CreateCode(this.instructions,
                    this.instructions.associatedMethod.ReturnType.GenericArguments().Any() ?
                       this.instructions.associatedMethod.ReturnType.GetGenericArgument(0) :
                       this.instructions.associatedMethod.ReturnType, defaultValue));
            }

            return this;
        }

        public Coder For(LocalVariable array, Action<Coder, LocalVariable> action)
        {
            var item = this.instructions.associatedMethod.GetOrCreateVariable(array.Type.ChildType);
            var indexer = this.instructions.associatedMethod.GetOrCreateVariable(typeof(int));
            var lengthCheck = this.instructions.ilprocessor.Create(OpCodes.Ldloc, indexer.variable);

            // var i = 0;
            this.instructions.Emit(OpCodes.Ldc_I4_0);
            this.instructions.Emit(OpCodes.Stloc, indexer.variable);
            this.instructions.Emit(OpCodes.Br, lengthCheck);

            var start = this.instructions.ilprocessor.Create(OpCodes.Nop);
            this.instructions.Append(start);

            this.instructions.Emit(OpCodes.Ldloc, array.variable);
            this.instructions.Emit(OpCodes.Ldloc, indexer.variable);
            this.instructions.Emit(LoadElement(array.variable.VariableType));
            this.instructions.Emit(OpCodes.Stloc, item.variable);

            action(this, item);

            // i++
            this.instructions.Emit(OpCodes.Ldloc, indexer.variable);
            this.instructions.Emit(OpCodes.Ldc_I4_1);
            this.instructions.Emit(OpCodes.Add);
            this.instructions.Emit(OpCodes.Stloc, indexer.variable);

            // i < array.Length
            this.instructions.Append(lengthCheck);
            this.instructions.Emit(OpCodes.Ldloc, array.variable);
            this.instructions.Emit(OpCodes.Ldlen);
            this.instructions.Emit(OpCodes.Conv_I4);
            this.instructions.Emit(OpCodes.Clt);
            this.instructions.Emit(OpCodes.Brtrue, start);

            return this;
        }

        public Position GetFirstOrDefaultPosition(Func<Instruction, bool> predicate)
        {
            if (this.instructions.associatedMethod.methodDefinition.Body == null || this.instructions.associatedMethod.methodDefinition.Body.Instructions == null)
                return null;

            foreach (var item in this.instructions.associatedMethod.methodDefinition.Body.Instructions)
                if (predicate(item))
                    return new Position(this.instructions.associatedMethod, item);

            return null;
        }

        public ParametersVariableCodeBlock GetParametersArray()
        {
            Method targetMethod;
            Method originMethod;

            if (this.instructions.associatedMethod.IsAsync)
            {
                targetMethod = this.instructions.associatedMethod;
                originMethod = this.instructions.associatedMethod.AsyncMethodHelper.Method;
            }
            else if (this.instructions.associatedMethod.AsyncOriginType.IsAsyncStateMachine)
            {
                targetMethod = this.instructions.associatedMethod;
                originMethod = this.instructions.associatedMethod.AsyncMethodHelper.Method;
            }
            else
            {
                targetMethod = this.instructions.associatedMethod;
                originMethod = this.instructions.associatedMethod;
            }

            var variableName = "<>params_" + targetMethod.Identification;
            var variable = targetMethod.GetVariable(variableName);

            if (variable == null)
                variable = targetMethod.GetVariable(targetMethod.Identification);

            if (this.AssociatedMethod is AsyncStateMachineMoveNextMethod moveNextMethod)
                moveNextMethod.BeginOfCode = this.instructions[0];

            if (variable == null)
            {
                var objectArrayType = Builder.Current.GetType(typeof(object[]));
                var newBlock = this.instructions.Spawn();
                variable = targetMethod.GetOrCreateVariable(objectArrayType, variableName);

                if (variable?.variable == null)
                    throw new NullReferenceException("Unable to create a local variable");

                newBlock.Emit(OpCodes.Ldc_I4, originMethod.methodReference.Parameters.Count);
                newBlock.Emit(OpCodes.Newarr, (objectArrayType.typeReference as ArrayType).ElementType);
                newBlock.Emit(OpCodes.Stloc, variable.variable);

                if (originMethod.IsAsync)
                {
                    int counter = 0;
                    foreach (var parameter in originMethod.methodReference.Parameters)
                    {
                        newBlock.Emit(OpCodes.Ldloc, variable.variable);
                        newBlock.Append(InstructionBlock.CreateCode(newBlock, null, counter++));
                        newBlock.Append(InstructionBlock.CreateCode(newBlock, BuilderType.Object, targetMethod.OriginType.GetField(parameter.Name)));
                        newBlock.Emit(OpCodes.Stelem_Ref);
                    }
                }
                else
                {
                    foreach (var parameter in originMethod.methodReference.Parameters)
                        newBlock.Append(IlHelper.ProcessParam(parameter, variable.variable));
                }

                // Insert the call in the beginning of the instruction list
                this.instructions.Insert(0, newBlock);

                return new ParametersVariableCodeBlock(variable.variable);
            }
            else
            {
                return new ParametersVariableCodeBlock(variable.variable);
            }
        }

        public bool HasReturnVariable()
        {
            if (this.instructions.associatedMethod.GetVariable(CodeBlocks.ReturnVariableName) != null)
                return true;

            if (this.instructions.associatedMethod.GetVariable("result") != null)
                return true;

            return false;
        }

        public T Load<T>(CecilatorBase cecilatorBase) where T : class
        {
            if (cecilatorBase is Field field) return this.Load(field) as T ?? throw new InvalidCastException($"Can't be casted to {typeof(T).FullName}");
            if (cecilatorBase is LocalVariable variable) return this.Load(variable) as T ?? throw new InvalidCastException($"Can't be casted to {typeof(T).FullName}");
            throw new NotImplementedException("This is only available for Field and LocalVariable");
        }

        public Coder Newarr(BuilderType type, int size)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this.instructions, null, size));
            this.instructions.Emit(OpCodes.Newarr, type.Import().typeReference);

            return this;
        }

        /// <summary>
        /// Copies the original body of the method to the <see cref="Coder"/> stack.
        /// </summary>
        /// <param name="createNewMethod">If true, creates a new method containing the original method's body; otherwise it will only weaved into the current <see cref="Coder"/> stack. Default is false.</param>
        /// <returns></returns>
        public Coder OriginalBody(bool createNewMethod = false)
        {
            if (this.AssociatedMethod is AsyncStateMachineMoveNextMethod moveNextMethod)
            {
                if (createNewMethod)
                    throw new NotSupportedException("Creating a new method is not supported for asyncronous methods.");

                this.instructions.Emit_Nop();

                var ldLocRequired = false;
                var firstInstruction = moveNextMethod.BeginOfCode ?? (this.instructions.Count == 0 ? null : this.instructions[0]);

                if (firstInstruction != null && firstInstruction.OpCode != OpCodes.Ldloc_0)
                {
                    var instructionToInsert = new Instruction[]
                        {
                            this.instructions.ilprocessor.Create(OpCodes.Ldloc_0),
                            this.instructions.ilprocessor.Create(OpCodes.Brfalse, this.instructions.Last),
                        };

                    this.instructions.Insert(firstInstruction, instructionToInsert);
                    ldLocRequired = true;
                }

                var originalInstructions = this.instructions.associatedMethod.methodDefinition.Body.Instructions;
                var asyncPositions = new AsyncStateMachinePositions(moveNextMethod.OriginMethod);
                var copiedInstructions = CopyMethodBody(this.instructions.associatedMethod.methodDefinition);
                this.instructions.Append(copiedInstructions.Get(asyncPositions.tryBegin, asyncPositions.tryEnd));
                this.instructions.Emit_Nop();

                for (int i = asyncPositions.tryBegin.Index; i < asyncPositions.tryEnd.Index; i++)
                {
                    if (originalInstructions[i].Operand is Instruction instruction &&
                        (instruction.Offset >= asyncPositions.tryEnd.instruction.Offset || instruction.Offset < asyncPositions.tryBegin.instruction.Offset))
                    {
                        if (instruction == asyncPositions.catchEnd.instruction)
                            copiedInstructions[i].Operand = this.instructions.Last;
                        else
                            moveNextMethod.OutOfBoundJumpIndex.Add(new Tuple<Instruction, int>(copiedInstructions[i], originalInstructions.IndexOf(instruction)));
                    }
                }

                if (ldLocRequired)
                {
                    var nop = this.instructions.ilprocessor.Create(OpCodes.Nop);
                    this.instructions.Append(new Instruction[]
                    {
                        this.instructions.ilprocessor.Create(OpCodes.Ldloc_0),
                        this.instructions.ilprocessor.Create(OpCodes.Brtrue, nop),
                        this.instructions.ilprocessor.Create(OpCodes.Leave, asyncPositions.catchEnd.instruction),
                        nop
                    });
                    moveNextMethod.OutOfBoundJumpIndex.Add(new Tuple<Instruction, int>(this.instructions[this.instructions.Count - 2], originalInstructions.IndexOf(asyncPositions.catchEnd.instruction)));
                }
            }
            else
            {
                if (createNewMethod)
                    return this.OriginalBodyNewMethod();

                var copiedInstructions = CopyMethodBody(this.instructions.associatedMethod.methodDefinition);

                // special case for .ctor
                if (this.instructions.associatedMethod.IsCtor)
                {
                    // remove everything until base call
                    var first = copiedInstructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                    if (first == null)
                        throw new NullReferenceException($"The constructor of type '{this.instructions.associatedMethod.OriginType}' seems to have no call to base class.");

                    var firstIndex = copiedInstructions.IndexOf(first);
                    copiedInstructions.RemoveRange(0, firstIndex);
                }

                this.instructions.Append(copiedInstructions);
            }

            return this;
        }

        public Coder SetValue(CecilatorBase cecilatorBase, Func<Coder, object> value)
        {
            if (cecilatorBase is Field field) return this.SetValue(field, value);
            if (cecilatorBase is LocalVariable variable) return this.SetValue(variable, value);
            throw new NotImplementedException("This is only available for Field and LocalVariable");
        }

        public Coder ThrowNew(Type exception)
        {
            this.instructions.Emit(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", 0)));
            this.instructions.Emit(OpCodes.Throw);
            return this;
        }

        public Coder ThrowNew(Type exception, string message)
        {
            this.instructions.Emit(OpCodes.Ldstr, message);
            this.instructions.Emit(OpCodes.Newobj, Builder.Current.Import(Builder.Current.Import(exception).GetMethodReference(".ctor", new Type[] { typeof(string) })));
            this.instructions.Emit(OpCodes.Throw);
            return this;
        }

        public Coder ThrowNew(Method ctor, params object[] parameters)
        {
            this.Append(InstructionBlock.NewObj(this, ctor, parameters));
            this.instructions.Emit(OpCodes.Throw);
            return this;
        }

        private static void SetCorrectJumpPoints(List<Index> jumps, IList<Instruction> methodInstructions)
        {
            foreach (var j in jumps.GroupBy(x => x.currentIndex))
            {
                if (methodInstructions[j.Key].OpCode == OpCodes.Switch)
                {
                    var instructions = new List<Instruction>();

                    foreach (var item in j)
                        instructions.Add(methodInstructions[item.index]);

                    methodInstructions[j.Key].Operand = instructions.ToArray();
                }
                else
                {
                    var index = j.First().index;
                    methodInstructions[j.Key].Operand = methodInstructions[index];
                }
            }
        }

        private void CopyMethod(MethodDefinition method)
        {
            var methodProcessor = method.Body.GetILProcessor();
            var instructions = CopyMethodBody(this.instructions.associatedMethod.methodDefinition, this.instructions.associatedMethod.methodDefinition.Body.Variables);
            methodProcessor.Append(instructions.Instructions);

            foreach (var item in instructions.Exceptions)
                method.Body.ExceptionHandlers.Add(item);
        }

        private InstructionCollection CopyMethodBody(MethodDefinition originalMethod, IList<VariableDefinition> variableDefinition)
        {
            if (this.instructions.associatedMethod.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            var methodProcessor = originalMethod.Body.GetILProcessor();
            var resultingInstructions = new List<InstructionOriginalTarget>();
            var exceptionList = new List<ExceptionHandler>();
            var jumps = new List<Index>();

            for (int i = 0; i < originalMethod.Body.Instructions.Count; i++)
            {
                var item = originalMethod.Body.Instructions[i];

                if (item.Operand is Instruction)
                    resultingInstructions.Add(new InstructionOriginalTarget(item, CreateInstruction(originalMethod.Body.Instructions, item, i, methodProcessor, ref jumps)));
                else if (item.Operand is Instruction[])
                {
                    var instructions = item.Operand as Instruction[];
                    var newInstructions = new Instruction[instructions.Length];
                    for (int x = 0; x < instructions.Length; x++)
                        newInstructions[x] = CreateInstruction(originalMethod.Body.Instructions, instructions[x], i, methodProcessor, ref jumps);

                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = newInstructions;

                    resultingInstructions.Add(new InstructionOriginalTarget(item, instruction));
                }
                //else if (item.Operand is CallSite )
                //    throw new NotImplementedException($"Unknown operand '{item.OpCode.ToString()}' '{item.Operand.GetType().FullName}'");
                else
                {
                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = item.Operand;
                    resultingInstructions.Add(new InstructionOriginalTarget(item, instruction));

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

            return new InstructionCollection
            {
                Instructions = resultingInstructions.Select(x => x.Target),
                Exceptions = exceptionList
            };
        }

        private InstructionBlock CopyMethodBody(MethodDefinition originalMethod)
        {
            if (originalMethod.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            var variableDefinition = originalMethod.Body.Variables;
            var methodProcessor = originalMethod.Body.GetILProcessor();
            var resultingInstructions = new List<InstructionOriginalTarget>();
            var exceptionList = new List<ExceptionHandler>();
            var jumps = new List<Index>();

            for (int i = 0; i < originalMethod.Body.Instructions.Count; i++)
            {
                var item = originalMethod.Body.Instructions[i];

                if (item.Operand is Instruction)
                    resultingInstructions.Add(new InstructionOriginalTarget(item, CreateInstruction(originalMethod.Body.Instructions, item, i, methodProcessor, ref jumps)));
                else if (item.Operand is Instruction[])
                {
                    var instructions = item.Operand as Instruction[];
                    var newInstructions = new Instruction[instructions.Length];
                    for (int x = 0; x < instructions.Length; x++)
                        newInstructions[x] = CreateInstruction(originalMethod.Body.Instructions, instructions[x], i, methodProcessor, ref jumps);

                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = newInstructions;

                    resultingInstructions.Add(new InstructionOriginalTarget(item, instruction));
                }
                //else if (item.Operand is CallSite )
                //    throw new NotImplementedException($"Unknown operand '{item.OpCode.ToString()}' '{item.Operand.GetType().FullName}'");
                else
                {
                    var instruction = methodProcessor.Create(OpCodes.Nop);
                    instruction.OpCode = item.OpCode;
                    instruction.Operand = item.Operand;
                    resultingInstructions.Add(new InstructionOriginalTarget(item, instruction));

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
            {
                var handler = copyHandler(item);

                if (handler.FilterStart == null &&
                    handler.HandlerEnd == null &&
                    handler.HandlerStart == null &&
                    handler.TryEnd == null &&
                    handler.TryStart == null)
                    continue;

                exceptionList.Add(handler);
            }

            var result = this.instructions.Spawn();
            result.Append(resultingInstructions.Select(x => x.Target), exceptionList);
            return result;
        }

        private Instruction CreateInstruction(IList<Instruction> instructions, Instruction instructionTarget, int currentIndex, ILProcessor processor, ref List<Index> jumps)
        {
            GetJumpPoints(instructions, instructionTarget, currentIndex, ref jumps);

            var instructionResult = processor.Create(OpCodes.Nop);
            instructionResult.OpCode = instructionTarget.OpCode;
            return instructionResult;
        }

        private void GetJumpPoints(IList<Instruction> instructions, Instruction instructionTarget, int currentIndex, ref List<Index> jumps)
        {
            var operand = instructionTarget.Operand as Instruction;

            if (operand == null)
                return;

            var index = instructions.IndexOf(operand);

            if (index >= 0)
                jumps.Add(new Index(currentIndex, index));
            else
            {
                index = instructions.IndexOf(operand.Offset);

                if (index >= 0)
                    jumps.Add(new Index(currentIndex, index));
            }
        }

        private OpCode LoadElement(TypeReference typeReference)
        {
            if (typeReference.AreEqual(BuilderType.Byte)) return OpCodes.Ldelem_U1;
            if (typeReference.AreEqual(BuilderType.SByte)) return OpCodes.Ldelem_I1;
            if (typeReference.AreEqual(BuilderType.Int16)) return OpCodes.Ldelem_I2;
            if (typeReference.AreEqual(BuilderType.UInt16)) return OpCodes.Ldelem_U2;
            if (typeReference.AreEqual(BuilderType.Int32)) return OpCodes.Ldelem_I4;
            if (typeReference.AreEqual(BuilderType.UInt32)) return OpCodes.Ldelem_U4;
            if (typeReference.AreEqual(BuilderType.Int64)) return OpCodes.Ldelem_I8;
            if (typeReference.AreEqual(BuilderType.UInt64)) return OpCodes.Ldelem_I8;
            if (typeReference.AreEqual(BuilderType.Single)) return OpCodes.Ldelem_R4;
            if (typeReference.AreEqual(BuilderType.Double)) return OpCodes.Ldelem_R8;
            if (typeReference.AreEqual(BuilderType.IntPtr)) return OpCodes.Ldelem_I;
            if (typeReference.AreEqual(BuilderType.UIntPtr)) return OpCodes.Ldelem_I;

            return OpCodes.Ldelem_Ref;
        }

        private Coder OriginalBodyNewMethod()
        {
            var newMethod = this.Copy(Modifiers.Private, $"<{this.instructions.associatedMethod.Name}>m__original");

            for (int i = 0; i < this.instructions.associatedMethod.Parameters.Length + (this.instructions.associatedMethod.IsStatic ? 0 : 1); i++)
                this.instructions.Append(this.instructions.ilprocessor.Create(OpCodes.Ldarg, i));

            this.instructions.Emit(OpCodes.Call, newMethod.Import());

            return this;
        }

        private struct Index
        {
            public int currentIndex;

            public int index;

            public Index(int currentIndex, int index)
            {
                this.currentIndex = currentIndex;
                this.index = index;
            }
        }

        private struct InstructionCollection
        {
            public IEnumerable<ExceptionHandler> Exceptions;
            public IEnumerable<Instruction> Instructions;
        }

        private struct InstructionOriginalTarget
        {
            public Instruction Original;

            public Instruction Target;

            public InstructionOriginalTarget(Instruction original, Instruction target)
            {
                this.Original = original;
                this.Target = target;
            }
        }

        #region Exit Operators

        public Coder Return()
        {
            this.ImplementReturn();
            return this;
        }

        #endregion Exit Operators

        #region Call Methods

        public CallCoder Call(Method method)
        {
            this.InternalCall(CodeBlocks.This, method);
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder Call(Method method, params object[] parameters)
        {
            this.InternalCall(CodeBlocks.This, method, parameters);
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            if (parameters == null)
            {
                this.InternalCall(CodeBlocks.This, method, new object[] { null });
                return new CallCoder(this, method.ReturnType);
            }

            this.InternalCall(CodeBlocks.This, method, this.CreateParameters(parameters));
            return new CallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region NewObj Methods

        public CallCoder NewObj(AttributedMethod attributedMethod)
        {
            this.NewObj(attributedMethod.customAttribute);
            return new CallCoder(this, attributedMethod.Attribute.Type);
        }

        public CallCoder NewObj(AttributedType attributedType)
        {
            this.NewObj(attributedType.customAttribute);
            return new CallCoder(this, attributedType.Attribute.Type);
        }

        public CallCoder NewObj(AttributedProperty attributedProperty)
        {
            this.NewObj(attributedProperty.customAttribute);
            return new CallCoder(this, attributedProperty.Attribute.Type);
        }

        public CallCoder NewObj(Method method) => this.NewObj(method, new object[0]);

        public CallCoder NewObj(Method method, params object[] parameters)
        {
            this.instructions.Append(InstructionBlock.NewObj(this.instructions, method, parameters));
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder NewObj(Method method, params Func<Coder, object>[] parameters) => this.NewObj(method, this.CreateParameters(parameters));

        #endregion NewObj Methods

        #region Field Operations

        public CallCoder Load(BuilderType builderType)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, null, builderType));
            return new CallCoder(this, builderType);
        }

        public FieldCoder Load(Field field)
        {
            InstructionBlock.CreateCodeForFieldReference(this, field.FieldType, field, true);
            return new FieldCoder(this, field.FieldType);
        }

        public FieldCoder Load(Func<BuilderType, Field> field) => Load(field(this.instructions.associatedMethod.type));

        public Coder SetValue(Field field, object value)
        {
            this.instructions.Append(InstructionBlock.SetValue(this, CodeBlocks.This, field, value));
            return this;
        }

        public Coder SetValue(Field field, Func<Coder, object> value)
        {
            if (value == null)
                return SetValue(field, (object)null);

            return SetValue(field, value(this.NewCoder()));
        }

        public Coder SetValue(Func<BuilderType, Field> field, object value) => this.SetValue(field(this.instructions.associatedMethod.type), value);

        public Coder SetValue(Func<BuilderType, Field> field, Func<Coder, object> value) => SetValue(field, value(this.NewCoder()));

        #endregion Field Operations

        #region Arg Operations

        public ArgCoder Load(ParametersCodeBlock arg)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("This kind of parameter is not supported by Load.");

            var argInfo = arg.GetTargetType(this.instructions.associatedMethod);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, arg));
            return new ArgCoder(this, argInfo.Item1);
        }

        public Coder SetValue(ParametersCodeBlock arg, object value)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("Setting value to all parameters at once is not supported");

            var argInfo = arg.GetTargetType(this);
            this.instructions.Append(InstructionBlock.CreateCode(this, argInfo.Item1, value));
            this.instructions.Emit(OpCodes.Starg, argInfo.Item3);
            return new Coder(this);
        }

        public Coder SetValue(ParametersCodeBlock arg, Func<Coder, object> value)
        {
            if (value == null)
                return this.SetValue(arg, (object)value);

            return this.SetValue(arg, value(this.NewCoder()));
        }

        #endregion Arg Operations

        #region Local Variable Operations

        public VariableCoder Load(LocalVariable variable)
        {
            InstructionBlock.CreateCodeForVariableDefinition(this, variable.Type, variable);
            return new VariableCoder(this, variable.Type);
        }

        public VariableCoder Load(Func<Method, LocalVariable> variable) => Load(variable(this.instructions.associatedMethod));

        public Coder SetValue(LocalVariable variable, object value)
        {
            this.instructions.Append(InstructionBlock.SetValue(this, variable, value));
            return this;
        }

        public Coder SetValue(LocalVariable variable, Func<Coder, object> value)
        {
            if (value == null)
                return this.SetValue(variable, (object)value);

            return SetValue(variable, value(this.NewCoder()));
        }

        public Coder SetValue(Func<Method, LocalVariable> variable, object value) => this.SetValue(variable(this.instructions.associatedMethod), value);

        public Coder SetValue(Func<Method, LocalVariable> variable, Func<Coder, object> value) => SetValue(variable, value(this.NewCoder()));

        #endregion Local Variable Operations

        #region Load Value

        public Coder Load(object value)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, null, value));
            return this;
        }

        #endregion Load Value

        #region Casting Operations

        public Coder As(BuilderType type)
        {
            if (this.instructions.associatedMethod.IsStatic)
                throw new NotSupportedException("This is not supported in static methods.");

            this.instructions.Emit(OpCodes.Ldarg_0);
            InstructionBlock.CastOrBoxValues(this, type);
            return this;
        }

        CoderBase ICasting.As(BuilderType type) => this.As(type);

        #endregion Casting Operations

        #region if Statements

        public Coder If(
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Func<Coder, object> then)
        {
            var result = booleanExpression(new BooleanExpressionCoder(this.NewCoder()));
            this.instructions.Append(result);
            this.instructions.Append(result.jumpTargets.beginning);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, then(this.NewCoder())));
            this.instructions.Append(result.jumpTargets.ending);

            return this;
        }

        public Coder If(
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Func<Coder, object> then,
            Func<Coder, object> @else)
        {
            var endOfIf = this.instructions.ilprocessor.Create(OpCodes.Nop);
            var result = booleanExpression(new BooleanExpressionCoder(this.NewCoder()));

            this.instructions.Append(result);
            this.instructions.Append(result.jumpTargets.beginning);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, then(this.NewCoder())));
            this.instructions.Append(this.instructions.ilprocessor.Create(OpCodes.Br, endOfIf));
            this.instructions.Append(result.jumpTargets.ending);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, @else(this.NewCoder())));
            this.instructions.Append(endOfIf);

            return this;
        }

        #endregion if Statements

        #region Builder

        public void Insert(InsertionAction action, Position position)
        {
            if (action == InsertionAction.After)
                this.instructions.ilprocessor.InsertAfter(position.instruction, this.instructions);
            else if (action == InsertionAction.Before)
                this.instructions.ilprocessor.InsertBefore(position.instruction, this.instructions);

            foreach (var item in this.instructions.exceptionHandlers)
                this.instructions.ilprocessor.Body.ExceptionHandlers.Add(item);

            // Add removal of unused variables here
            ReplaceReturns(this);
            RemoveNops(this.AssociatedMethod);
            CleanLocalVariableList(this);
            this.instructions.associatedMethod.methodDefinition.Body.OptimizeMacros();
            this.instructions.associatedMethod.methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > 0;
            this.instructions.Clear();
        }

        public void Insert(InsertionPosition position)
        {
            Instruction instructionPosition = null;
            if (this.instructions.ilprocessor.Body == null || this.instructions.ilprocessor.Body.Instructions.Count == 0)
                this.instructions.Emit(OpCodes.Ret);

            if (position == InsertionPosition.CtorBeforeInit)
            {
                instructionPosition = this.instructions.ilprocessor.Body.Instructions[0];
            }
            else if (position == InsertionPosition.Beginning)
            {
                if (this.instructions.associatedMethod.IsCtor)
                    instructionPosition = InstructionBlock.GetCtorBaseOrThisCall(this.instructions.associatedMethod.methodDefinition);
                else
                    instructionPosition = this.instructions.ilprocessor.Body.Instructions[0];
            }
            else
            {
                if (this.instructions.associatedMethod.IsCCtor) instructionPosition = this.instructions.ilprocessor.Body.Instructions.Last();
                else if (this.instructions.associatedMethod.IsCtor) instructionPosition = InstructionBlock.GetCtorBaseOrThisCall(this.instructions.associatedMethod.methodDefinition);
                else
                {
                    var last = this.instructions.ilprocessor.Body.Instructions.Last();
                    var jumpers = this.instructions.ilprocessor.Body.Instructions.GetJumpSources(last.Previous);

                    if (!last.Previous.IsLoadLocal() && this.instructions.associatedMethod.methodDefinition.ReturnType.AreEqual(BuilderType.Void))
                    {
                        var isInitialized = this.instructions.associatedMethod.methodDefinition.Body.InitLocals;
                        var localVariable = this.GetOrCreateReturnVariable();

                        this.instructions.ilprocessor.InsertBefore(last, this.instructions.ilprocessor.Create(OpCodes.Stloc, localVariable));
                        this.instructions.ilprocessor.InsertBefore(last, this.instructions.ilprocessor.Create(OpCodes.Ldloc, localVariable));
                    }

                    instructionPosition = last.Previous;

                    foreach (var item in jumpers)
                        item.Operand = this.instructions.FirstOrDefault();
                }
            }

            this.instructions.ilprocessor.InsertBefore(instructionPosition, this.instructions);

            foreach (var item in this.instructions.exceptionHandlers)
                this.instructions.ilprocessor.Body.ExceptionHandlers.Add(item);

            // Add removal of unused variables here

            ReplaceReturns(this);
            RemoveNops(this.AssociatedMethod);
            CleanLocalVariableList(this);
            this.instructions.associatedMethod.methodDefinition.Body.OptimizeMacros();
            this.instructions.associatedMethod.methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > 0;

            this.instructions.Clear();
        }

        public Positions Replace(Position position)
        {
            if (this.AssociatedMethod is AsyncStateMachineMoveNextMethod)
                throw new NotSupportedException("Replace at position is not supported for async methods.");

            if (position.instruction == null)
                throw new ArgumentNullException(nameof(position.instruction));

            var index = this.instructions.associatedMethod.methodDefinition.Body.Instructions.IndexOf(position.instruction);
            this.instructions.ilprocessor.Remove(position.instruction);
            this.instructions.ilprocessor.InsertBefore(index, this.instructions);

            return new Positions(new Position(this.instructions.associatedMethod, this.instructions[0]), new Position(this.instructions.associatedMethod, this.instructions.LastOrDefault()));
        }

        /// <summary>
        /// Replaces the current methods body with the <see cref="Instruction"/>s in the <see cref="Coder"/>'s instruction set.
        /// </summary>
        public void Replace()
        {
            // A very special case for async methods
            if (this.instructions.associatedMethod is AsyncStateMachineMoveNextMethod moveNextMethod)
            {
                var positions = new AsyncStateMachinePositions(moveNextMethod.OriginMethod);

                this.instructions.exceptionHandlers.Add(positions.exceptionHandler);

                var instruction = positions.exceptionHandler.TryStart;
                do
                {
                    this.instructions.Insert(0, instruction);
                    instruction = instruction.Previous;
                }
                while (instruction != null);
                instruction = positions.tryEnd.instruction;
                do
                {
                    this.instructions.Append(instruction);
                    instruction = instruction.Next;
                } while (instruction != null);
                positions.exceptionHandler.TryStart.OpCode = OpCodes.Nop;
                positions.exceptionHandler.TryStart.Operand = null;

                // Replace all out of bound jumps with its proper jump targets
                foreach (var item in moveNextMethod.OutOfBoundJumpIndex)
                {
                    var targetInstruction = this.instructions.associatedMethod.methodDefinition.Body.Instructions[item.Item2];
                    item.Item1.Operand = targetInstruction;
                }

                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(this.instructions);
            }
            else /* Special case for .ctors */
                if (this.instructions.associatedMethod.IsCtor &&
                    this.instructions.associatedMethod.methodDefinition.Body?.Instructions != null &&
                    this.instructions.associatedMethod.methodDefinition.Body.Instructions.Count > 0)
            {
                var first = this.instructions.associatedMethod.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                if (first == null)
                    throw new NullReferenceException($"The constructor of type '{this.instructions.associatedMethod.OriginType}' seems to have no call to base class.");

                // In ctors we only replace the instructions after base call
                var callsBeforeBase = this.instructions.associatedMethod.methodDefinition.Body.Instructions.TakeWhile(x => x != first).ToList();
                callsBeforeBase.Add(first);

                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(callsBeforeBase);
                this.instructions.ilprocessor.Append(this.instructions);
            }
            else
            {
                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(this.instructions);
            }

            foreach (var item in this.instructions.exceptionHandlers)
                this.instructions.ilprocessor.Body.ExceptionHandlers.Add(item);

            ReplaceReturns(this);
            RemoveNops(this.AssociatedMethod);
            CleanLocalVariableList(this);
            this.instructions.associatedMethod.methodDefinition.Body.OptimizeMacros();
            this.instructions.associatedMethod.methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > 0;
            this.instructions.Clear();
        }

        private static void CleanLocalVariableList(Coder coder)
        {
            // TODO
            //var variables = new List<(int index, VariableDefinition variable, bool required)>();
            //var varCollection = coder.AssociatedMethod.methodDefinition.Body.Variables;

            //for (int i = 0; i < varCollection.Count; i++)
            //    variables.Add((i, varCollection[i], false));

            //// Expand all ldloc
            //foreach (var instruction in coder.AssociatedMethod.methodDefinition.Body.Instructions)
            //{
            //    if (instruction.OpCode == OpCodes.Ldloc_0)
            //    {
            //        instruction.OpCode = OpCodes.Ldloc;
            //        instruction.Operand = varCollection[0];
            //    }
            //    else if (instruction.OpCode == OpCodes.Ldloc_1)
            //    {
            //        instruction.OpCode = OpCodes.Ldloc;
            //        instruction.Operand = varCollection[1];
            //    }
            //    else if (instruction.OpCode == OpCodes.Ldloc_2)
            //    {
            //        instruction.OpCode = OpCodes.Ldloc;
            //        instruction.Operand = varCollection[2];
            //    }
            //    else if (instruction.OpCode == OpCodes.Ldloc_3)
            //    {
            //        instruction.OpCode = OpCodes.Ldloc;
            //        instruction.Operand = varCollection[3];
            //    }
            //    else if (instruction.OpCode == OpCodes.ldlo)
            //    {
            //        instruction.OpCode = OpCodes.Ldloc;
            //        instruction.Operand = varCollection[4];
            //    }
            //}
        }

        private static void RemoveNops(Method method)
        {
            if (method.IsAbstract)
                return;

            var instructions = method.methodDefinition.Body.Instructions;

            Instruction GetNextNonNop(Instruction instruction)
            {
                if (instruction == null)
                    return null;

                if (instruction.OpCode != OpCodes.Nop)
                    return instruction;

                var temp = instruction;
                do
                {
                    if (temp.OpCode != OpCodes.Nop)
                        return temp;

                    temp = temp.Next;
                } while (temp != null);

                temp = instruction;
                do
                {
                    if (temp.OpCode != OpCodes.Nop)
                        return temp;

                    temp = temp.Previous;
                } while (temp != null);

                return instruction;
            }

            foreach (var item in method.methodDefinition.Body.ExceptionHandlers)
            {
                item.FilterStart = GetNextNonNop(item.FilterStart);
                item.HandlerEnd = GetNextNonNop(item.HandlerEnd);
                item.HandlerStart = GetNextNonNop(item.HandlerStart);
                item.TryEnd = GetNextNonNop(item.TryEnd);
                item.TryStart = GetNextNonNop(item.TryStart);
            }

            foreach (var item in instructions)
                if (item.Operand is Instruction o && o.OpCode == OpCodes.Nop)
                    item.Operand = GetNextNonNop(o);

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].OpCode != OpCodes.Nop)
                    continue;

                instructions.RemoveAt(i);
                i--;
            }
        }

        private static void ReplaceJumps(Method method, Instruction tobeReplaced, Instruction replacement)
        {
            for (var i = 0; i < method.methodDefinition.Body.Instructions.Count - 1; i++)
            {
                var instruction = method.methodDefinition.Body.Instructions[i];

                if (instruction.Operand == tobeReplaced)
                    instruction.Operand = replacement;
            }

            for (var i = 0; i < method.methodDefinition.Body.ExceptionHandlers.Count; i++)
            {
                var handler = method.methodDefinition.Body.ExceptionHandlers[i];

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

        private static void ReplaceReturns(Coder coder)
        {
            if (coder.instructions.Count == 0)
                return;

            if (coder.instructions.associatedMethod.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            bool ReplaceWithLeave(Instruction instruction)
            {
                if (!coder.instructions.associatedMethod.IsInclosedInHandlers(instruction))
                    return false;

                if (instruction.OpCode == OpCodes.Br || instruction.OpCode == OpCodes.Br_S)
                {
                    // A break that jumps to an address in the handler is ok
                    if (coder.instructions.associatedMethod.IsInclosedInHandlers(instruction.Operand as Instruction))
                        return false;

                    return true;
                }

                if (instruction.OpCode == OpCodes.Ret)
                    return true;

                return false;
            }

            if (coder.instructions.associatedMethod.IsVoid || coder.instructions.Last?.OpCode != OpCodes.Ret)
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (ReplaceWithLeave(instruction))
                    {
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = realReturn;
                    }
                }
            }
            else
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsValueOpCode() && realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    var previousReturn = realReturn.Previous;
                    resultJump = true;
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    coder.instructions.ilprocessor.InsertBefore(realReturn,
                        InstructionBlock.CreateCode(coder.instructions, coder.instructions.associatedMethod.ReturnType, coder.GetOrCreateReturnVariable()));
                    realReturn = previousReturn;
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

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (ReplaceWithLeave(instruction))
                    {
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = realReturn;

                        if (coder.instructions.associatedMethod.ReturnType == BuilderType.Void ||
                            coder.instructions.associatedMethod.methodDefinition.ReturnType.FullName == "System.Threading.Task" /* This should stay so that the Task type is not imported */)
                            continue;

                        if (resultJump)
                        {
                            var returnVariable = coder.GetOrCreateReturnVariable();
                            var previousInstruction = instruction.Previous;

                            if (previousInstruction != null && previousInstruction.IsLoadLocal())
                            {
                                if (
                                    (returnVariable.Index == 0 && previousInstruction.OpCode == OpCodes.Ldloc_0) ||
                                    (returnVariable.Index == 1 && previousInstruction.OpCode == OpCodes.Ldloc_1) ||
                                    (returnVariable.Index == 2 && previousInstruction.OpCode == OpCodes.Ldloc_2) ||
                                    (returnVariable.Index == 3 && previousInstruction.OpCode == OpCodes.Ldloc_3) ||
                                    (previousInstruction.OpCode == OpCodes.Ldloc_S && returnVariable.Index == (int)previousInstruction.Operand) ||
                                    (returnVariable.variable == previousInstruction.Operand as VariableDefinition)
                                    )
                                {
                                    ReplaceJumps(coder.instructions.associatedMethod, previousInstruction, instruction);

                                    // In this case also remove the redundant ldloc opcode
                                    i--;
                                    coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Remove(previousInstruction);
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
                                    (returnVariable.variable == previousInstruction.Operand as VariableDefinition)
                                    )
                                    continue; // Just continue and do not add an additional store opcode
                            }

                            //if (!coder.instructions.associatedMethod.IsInclosedInHandlers(instruction, ExceptionHandlerType.Finally))
                            coder.instructions.ilprocessor.InsertBefore(instruction, coder.instructions.ilprocessor.Create(OpCodes.Stloc, returnVariable.variable));
                        }
                    }
                }
            }
        }

        #endregion Builder

        #region Try Catch Finally

        public TryCoder Try(Func<Coder, Coder> code)
        {
            if (this.instructions.Count == 0)
                this.instructions.Emit(OpCodes.Nop);

            var result = new TryCoder(this);

            this.instructions.Append(code(this.NewCoder()));

            if (result.RequiresReturn)
            {
                if (this.AssociatedMethod is AsyncStateMachineMoveNextMethod moveNextMethod)
                {
                    var asyncPositions = new AsyncStateMachinePositions(moveNextMethod.OriginMethod);
                    this.instructions.Emit(OpCodes.Leave, asyncPositions.catchEnd.instruction);
                    moveNextMethod.OutOfBoundJumpIndex.Add(
                        new Tuple<Instruction, int>(
                            this.instructions[this.instructions.Count - 2],
                            this.AssociatedMethod.methodDefinition.Body.Instructions.IndexOf(asyncPositions.catchEnd.instruction)));
                }
                else
                    this.instructions.Emit(OpCodes.Ret);
            }

            return result;
        }

        #endregion Try Catch Finally
    }
}