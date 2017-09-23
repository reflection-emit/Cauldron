using System;
using System.Reflection;

namespace Cauldron.Interception.External.Test
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExternalLockableMethodAttribute : Attribute, IMethodInterceptor
    {
        private object lockObject = new object();

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            lock (lockObject)
            {
            }
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}