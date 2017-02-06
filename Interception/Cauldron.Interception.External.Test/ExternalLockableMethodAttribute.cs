using Cauldron.Core.Interceptors;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Cauldron.Interception.External.Test
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExternalLockableMethodAttribute : Attribute, ILockableMethodInterceptor
    {
        public void OnEnter(SemaphoreSlim semaphore, Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            semaphore.Wait();
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }
}