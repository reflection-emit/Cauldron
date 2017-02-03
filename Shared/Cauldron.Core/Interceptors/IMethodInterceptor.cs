using System;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    public interface IMethodInterceptor
    {
        void OnEnter(object instance, MethodBase methodbase);

        void OnException(Exception e);

        void OnExit();
    }
}