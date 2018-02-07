using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IVariableOperations<T>
    {
        T Load(LocalVariable variable);

        T Load(Func<Method, LocalVariable> variable);
    }

    public interface IVariableOperationsExtended<T> : IVariableOperations<T>
    {
        Coder SetValue(LocalVariable variable, object value);

        Coder SetValue(LocalVariable variable, Func<Coder, object> value);

        Coder SetValue(Func<Method, LocalVariable> variable, object value);

        Coder SetValue(Func<Method, LocalVariable> variable, Func<Coder, object> value);
    }
}