using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface INewObj<T>
    {
        T NewObj(Method method);

        T NewObj(Method method, params object[] parameters);

        T NewObj(Method method, params Func<Coder, object>[] parameters);
    }
}