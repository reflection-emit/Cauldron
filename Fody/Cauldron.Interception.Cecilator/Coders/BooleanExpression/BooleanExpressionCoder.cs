using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionCoder :
        CoderBase<BooleanExpressionCoder, BooleanExpressionCoder>,
        ICallMethod<BooleanExpressionCallCoder>,
        IFieldOperations<BooleanExpressionFieldCoder>,
        IVariableOperations<BooleanExpressionVariableCoder>,
        IArgOperationsExtended<BooleanExpressionArgCoder>,
        ICasting<BooleanExpressionCoder>,
        INewObj<BooleanExpressionCallCoder>
    {
        internal readonly Instruction jumpTarget;

        internal BooleanExpressionCoder(InstructionBlock instructionBlock, Instruction jumpTarget) : base(instructionBlock)
            => this.jumpTarget = jumpTarget;

        internal BooleanExpressionCoder(InstructionBlock instructionBlock) : base(instructionBlock)
            => this.jumpTarget = instructionBlock.ilprocessor.Create(OpCodes.Nop);

        public static implicit operator InstructionBlock(BooleanExpressionCoder coder) => coder.instructions;

        #region Call Methods

        public BooleanExpressionCallCoder Call(Method method)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method, this.CreateParameters(parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the declaring type.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method, parameters);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region NewObj Methods

        public BooleanExpressionCallCoder NewObj(Method method)
        {
            this.NewObj(method);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder NewObj(Method method, params object[] parameters)
        {
            this.NewObj(method, parameters);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder NewObj(Method method, params Func<Coder, object>[] parameters)
        {
            this.NewObj(method, this.CreateParameters(parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        #endregion NewObj Methods

        #region Arg Operations

        public BooleanExpressionArgCoder Load(ParametersCodeBlock arg)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("This kind of parameter is not supported by Load.");

            var argInfo = arg.GetTargetType(this.instructions.associatedMethod);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, arg));
            return new BooleanExpressionArgCoder(this, argInfo.Item1);
        }

        public Coder SetValue(ParametersCodeBlock arg, object value)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("Setting value to all parameters at once is not supported");

            this.instructions.Emit(OpCodes.Starg, arg);
            return new Coder(this);
        }

        public Coder SetValue(ParametersCodeBlock arg, Func<Coder, object> value) => this.SetValue(arg, value(this.NewCoder()));

        #endregion Arg Operations

        #region Field Operations

        public BooleanExpressionFieldCoder Load(Field field)
        {
            InstructionBlock.CreateCodeForFieldReference(this, field.FieldType, field, true);
            return new BooleanExpressionFieldCoder(this, field.FieldType);
        }

        public BooleanExpressionFieldCoder Load(Func<BuilderType, Field> field) => Load(field(this.instructions.associatedMethod.type));

        #endregion Field Operations

        #region Local Variable Operations

        public BooleanExpressionVariableCoder Load(LocalVariable variable)
        {
            InstructionBlock.CreateCodeForVariableDefinition(this, variable.Type, variable);
            return new BooleanExpressionVariableCoder(this, variable.Type);
        }

        public BooleanExpressionVariableCoder Load(Func<Method, LocalVariable> variable) => Load(variable(this.instructions.associatedMethod));

        #endregion Local Variable Operations

        #region Casting Operations

        public BooleanExpressionCoder As(BuilderType type)
        {
            if (this.instructions.associatedMethod.IsStatic)
                throw new NotSupportedException("This is not supported in static methods.");

            this.instructions.Emit(OpCodes.Ldarg_0);
            InstructionBlock.CastOrBoxValues(this, type);
            return new BooleanExpressionCoder(this, this.jumpTarget);
        }

        #endregion Casting Operations
    }
}