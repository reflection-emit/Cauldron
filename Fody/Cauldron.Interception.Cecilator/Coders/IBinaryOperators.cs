using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IBinaryOperators<TSelf>
    {
        TSelf And(Func<Coder, object> other);

        TSelf And(Field field);

        TSelf And(LocalVariable variable);

        TSelf And(ParametersCodeBlock arg);

        TSelf Invert();

        TSelf Or(Field field);

        TSelf Or(LocalVariable variable);

        TSelf Or(Func<Coder, object> other);

        TSelf Or(ParametersCodeBlock arg);
    }
}