using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IBitwiseOperators<TResult, TBooleanExpressionCoder>
        where TBooleanExpressionCoder : BooleanExpressionContextCoder
    {
        TResult And(Func<TBooleanExpressionCoder, TResult> other);

        TResult And(Field field);

        TResult And(LocalVariable variable);

        TResult Invert();

        TResult Or(Field field);

        TResult Or(LocalVariable variable);

        TResult Or(Func<TBooleanExpressionCoder, TResult> other);
    }
}