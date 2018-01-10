using Cauldron.Interception;
using System;
using System.Reflection;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TestMethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }
    }
}