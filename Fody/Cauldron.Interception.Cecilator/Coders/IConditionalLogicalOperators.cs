using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IConditionalLogicalOperators
    {
        BooleanExpressionResultCoder AndAnd(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other);

        BooleanExpressionResultCoder OrOr(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other);
    }
}