using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IArgOperations<T>
    {
        T Load(ParametersCodeBlock arg);
    }

    public interface IArgOperationsExtended<T> : IArgOperations<T>
    {
        Coder SetValue(ParametersCodeBlock arg, object value);

        Coder SetValue(ParametersCodeBlock arg, Func<Coder, object> value);
    }
}