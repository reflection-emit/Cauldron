using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IBitwiseOperators<TResult>
    {
        TResult And(Func<BooleanExpressionCoder, TResult> other);

        TResult And(Field field);

        TResult And(LocalVariable variable);

        TResult Invert();

        TResult Or(Field field);

        TResult Or(LocalVariable variable);

        TResult Or(Func<BooleanExpressionCoder, TResult> other);
    }
}