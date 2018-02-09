using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IMathOperators<TSelf>
    {
        TSelf Add(object value);

        TSelf Add(Func<Coder, object> value);

        TSelf Div(object value);

        TSelf Div(Func<Coder, object> value);

        TSelf Mul(object value);

        TSelf Mul(Func<Coder, object> value);

        TSelf Negate();

        TSelf Sub(object value);

        TSelf Sub(Func<Coder, object> value);
    }
}