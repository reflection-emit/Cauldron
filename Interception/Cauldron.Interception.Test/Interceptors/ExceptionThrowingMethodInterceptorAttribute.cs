using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExceptionThrowingMethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}