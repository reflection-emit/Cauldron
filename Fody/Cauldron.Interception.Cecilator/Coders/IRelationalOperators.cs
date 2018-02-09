using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IRelationalOperators
    {
        BooleanExpressionResultCoder Is(Func<Coder, object> value);

        BooleanExpressionResultCoder Is(Type type);

        BooleanExpressionResultCoder Is(BuilderType type);

        BooleanExpressionResultCoder Is(object value);

        BooleanExpressionResultCoder IsNot(object value);

        BooleanExpressionResultCoder IsNot(Type type);

        BooleanExpressionResultCoder IsNot(BuilderType type);

        BooleanExpressionResultCoder IsNot(Func<Coder, object> value);

        BooleanExpressionResultCoder IsNotNull();

        BooleanExpressionResultCoder IsNull();
    }
}