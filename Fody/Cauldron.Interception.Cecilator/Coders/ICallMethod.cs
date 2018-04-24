using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface ICallMethod<T>
    {
        T Call(Method method);

        T Call(Method method, params object[] parameters);

        T Call(Method method, params Func<Coder, object>[] parameters);
    }
}