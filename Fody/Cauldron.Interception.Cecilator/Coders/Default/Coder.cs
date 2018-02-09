using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class Coder :
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

        public bool HasReturnVariable() => this.instructions.associatedMethod.GetVariable(CodeBlocks.ReturnVariableName) != null;

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

        #region Exit Operators

        public Coder Return()
        {
            this.instructions.Emit(OpCodes.Ret);
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
            this.InternalCall(CodeBlocks.This, method, this.CreateParameters(parameters));
            return new CallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region NewObj Methods

        public CallCoder NewObj(Method method) => this.NewObj(method, new object[0]);

        public CallCoder NewObj(Method method, params object[] parameters)
        {
            this.instructions.Append(InstructionBlock.NewObj(this.instructions, method, parameters));
            return new CallCoder(this, method.ReturnType);
        }

        public CallCoder NewObj(Method method, params Func<Coder, object>[] parameters) => this.NewObj(method, this.CreateParameters(parameters));

        #endregion NewObj Methods

        #region Field Operations

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

        public Coder SetValue(Field field, Func<Coder, object> value) => SetValue(field, value(this.NewCoder()));

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

        public Coder SetValue(ParametersCodeBlock arg, Func<Coder, object> value) => this.SetValue(arg, value(this.NewCoder()));

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

        public Coder SetValue(LocalVariable variable, Func<Coder, object> value) => SetValue(variable, value(this.NewCoder()));

        public Coder SetValue(Func<Method, LocalVariable> variable, object value) => this.SetValue(variable(this.instructions.associatedMethod), value);

        public Coder SetValue(Func<Method, LocalVariable> variable, Func<Coder, object> value) => SetValue(variable, value(this.NewCoder()));

        #endregion Local Variable Operations

        #region Load Value

        public Coder Load(object value)
        {
            this.instructions.Append(InstructionBlock.CreateCode(this, null, value));

            if (value != null && value is ArrayCodeBlock arrayCodeSet)
            {
                this.instructions.Append(InstructionBlock.CreateCode(this.instructions, null, arrayCodeSet.index));
                this.instructions.Emit(OpCodes.Ldelem_Ref);
            }

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

        #endregion Casting Operations

        #region Special coder stuff

        /// <summary>
        /// Gets or creates a return variable.
        /// This will try to detect the existing return variable and create a new return variable if not found.
        /// </summary>
        /// <param name="coder">The coder to use.</param>
        /// <returns>A return variable.</returns>
        public VariableDefinition GetOrCreateReturnVariable()
        {
            var variable = this.instructions.associatedMethod.GetVariable(CodeBlocks.ReturnVariableName);

            if (variable != null)
                return variable;

            if (this.instructions.associatedMethod.methodDefinition.Body.Instructions.Count > 1)
            {
                var lastOpCode = this.instructions.associatedMethod.methodDefinition.Body.Instructions.Last().Previous;

                if (lastOpCode.IsLoadLocal())
                {
                    if (lastOpCode.Operand is int index && this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > index)
                        variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[index];

                    if (variable == null && lastOpCode.Operand is VariableDefinition variableReference)
                        variable = variableReference;

                    if (variable == null)
                        if (lastOpCode.OpCode == OpCodes.Ldloc_0) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[0];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_1) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[1];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_2) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[2];
                        else if (lastOpCode.OpCode == OpCodes.Ldloc_3) variable = this.instructions.associatedMethod.methodDefinition.Body.Variables[3];

                    if (variable != null)
                    {
                        this.instructions.associatedMethod.AddLocalVariable(CodeBlocks.ReturnVariableName, variable);
                        return variable;
                    }
                }
            }

            return this.instructions.associatedMethod.AddLocalVariable(CodeBlocks.ReturnVariableName, new VariableDefinition(this.instructions.associatedMethod.ReturnType.typeReference));
        }

        #endregion Special coder stuff

        #region if Statements

        public Coder If(
            Func<BooleanExpressionCoder, BooleanExpressionResultCoder> booleanExpression,
            Func<Coder, object> then)
        {
            var result = booleanExpression(new BooleanExpressionCoder(this.NewCoder()));
            this.instructions.Append(result);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, then(this.NewCoder())));
            this.instructions.Append(result.jumpTarget);

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
            this.instructions.Append(InstructionBlock.CreateCode(this, null, then(this.NewCoder())));
            this.instructions.Append(this.instructions.ilprocessor.Create(OpCodes.Br, endOfIf));
            this.instructions.Append(result.jumpTarget);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, @else(this.NewCoder())));
            this.instructions.Append(endOfIf);

            return this;
        }

        #endregion if Statements

        #region Builder

        /// <summary>
        /// Replaces the current methods body with the <see cref="Instruction"/>s in the <see cref="Coder"/>'s instruction set.
        /// </summary>
        /// <param name="coder"></param>
        public void Replace()
        {
            // Special case for .ctors
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
                this.instructions.ilprocessor.Append(this.instructions.instructions);
            }
            else
            {
                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(this.instructions.instructions);
            }

            foreach (var item in this.instructions.exceptionHandlers)
                this.instructions.ilprocessor.Body.ExceptionHandlers.Add(item);

            ReplaceReturns(this);

            // TODO: Add a method that removes unused variables this.CleanLocalVariableList();
            this.instructions.associatedMethod.methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > 0;

            this.instructions.associatedMethod.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
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

            if (coder.instructions.associatedMethod.IsVoid || coder.instructions.instructions.LastOrDefault().OpCode != OpCodes.Ret)
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = coder.instructions.associatedMethod.IsInclosedInHandlers(instruction) ? OpCodes.Leave : OpCodes.Br;
                    instruction.Operand = realReturn;
                }
            }
            else
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsValueOpCode() && realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    resultJump = true;
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    coder.instructions.ilprocessor.InsertBefore(realReturn,
                        InstructionBlock.CreateCode(coder.instructions, coder.instructions.associatedMethod.ReturnType, coder.GetOrCreateReturnVariable())
                            .instructions);

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

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    if (coder.instructions.associatedMethod.IsInclosedInHandlers(instruction))
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
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
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
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
                                    )
                                    continue; // Just continue and do not add an additional store opcode
                            }

                            coder.instructions.ilprocessor.InsertBefore(instruction, coder.instructions.ilprocessor.Create(OpCodes.Stloc, returnVariable));
                        }
                    }
                }
            }
        }

        #endregion Builder
    }
}