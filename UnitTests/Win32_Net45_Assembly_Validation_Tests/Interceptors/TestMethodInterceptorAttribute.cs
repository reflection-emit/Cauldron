using Cauldron.Interception;
using System;
using System.Reflection;

namespace Win32_Net45_Assembly_Validation_Tests.Interceptors
{
    public class TestMethodInterceptorAttribute : Attribute, IMethodInterceptor
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