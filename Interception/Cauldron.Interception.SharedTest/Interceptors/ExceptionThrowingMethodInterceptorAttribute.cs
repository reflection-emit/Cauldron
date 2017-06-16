using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExceptionThrowingMethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new Exception();
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}