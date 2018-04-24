using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IConditionalLogicalOperators
    {
        BooleanExpressionResultCoder AndAnd(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other);

        BooleanExpressionResultCoder AndAnd(Field other);

        BooleanExpressionResultCoder AndAnd(LocalVariable other);

        BooleanExpressionResultCoder AndAnd(ParametersCodeBlock other);

        BooleanExpressionResultCoder OrOr(Func<BooleanExpressionCoder, BooleanExpressionResultCoder> other);

        BooleanExpressionResultCoder OrOr(Field other);

        BooleanExpressionResultCoder OrOr(LocalVariable other);

        BooleanExpressionResultCoder OrOr(ParametersCodeBlock other);
    }
}