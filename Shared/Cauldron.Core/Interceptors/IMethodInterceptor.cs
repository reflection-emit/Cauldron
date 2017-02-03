using System;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    public interface IMethodInterceptor
    {
        void OnEnter(object instance, MethodBase methodbase, object[] values);

        void OnException(Exception e);

        void OnExit();
    }
}