using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionCallCoder :
        CoderBase<BooleanExpressionCallCoder, BooleanExpressionCoder>,
        ICallMethod<BooleanExpressionCallCoder>
    {
        private readonly BuilderType builderType;

        internal BooleanExpressionCallCoder(InstructionBlock instructionBlock, BuilderType builderType) : base(instructionBlock) => this.builderType = builderType;

        public override BooleanExpressionCoder End => new BooleanExpressionCoder(this);

        #region Call Methods

        public BooleanExpressionCallCoder Call(Method method)
        {
            this.InternalCall(null, method);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            this.InternalCall(null, method, parameters);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            this.InternalCall(null, method, this.CreateParameters(parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        public static implicit operator InstructionBlock(BooleanExpressionCallCoder coder) => coder.instructions;
    }
}