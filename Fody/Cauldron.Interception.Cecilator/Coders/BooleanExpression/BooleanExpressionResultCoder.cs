using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionResultCoder : BooleanExpressionCoderBase, IConditionalLogicalOperators
    {
        internal BooleanExpressionResultCoder(BooleanExpressionCoderBase coder) : base(coder.instructions, coder.jumpTarget, coder.builderType)
        {
        }

        public static implicit operator InstructionBlock(BooleanExpressionResultCoder coder) => coder.instructions;

        public BooleanExpressionResultCoder AndAnd(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other) => other(new BooleanExpressionCoder(this.instructions, this.jumpTarget));

        public BooleanExpressionResultCoder OrOr(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other)
        {
            throw new NotImplementedException();
        }
    }
}