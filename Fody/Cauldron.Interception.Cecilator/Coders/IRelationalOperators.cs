using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IRelationalOperators<TResult, TExpressionCoder, TCallCoder> : IRelationalOperators<TResult>
        where TExpressionCoder : ContextCoder
        where TCallCoder : ContextCoder
    {
        TResult EqualsTo(object value);

        TResult EqualsTo(Func<TExpressionCoder, TCallCoder> call);

        TResult EqualsTo(Field field);

        TResult NotEqualsTo(Field field);

        TResult NotEqualsTo(object value);
    }

    public interface IRelationalOperators<TResult>
    {
        TResult Is(Type type);

        TResult Is(BuilderType type);

        TResult IsFalse();

        TResult IsNotNull();

        TResult IsNull();

        TResult IsTrue();
    }
}