using Cauldron.Interception;
using System;
using System.Reflection;

namespace Fody_Assembly_Validation_External_Resources
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExternalMethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public ExternalMethodInterceptorAttribute(string message, int length)
        {
        }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }
    }
}