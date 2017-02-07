using Cauldron.Core.Interceptors;
using System;
using System.Threading;

namespace Cauldron.Interception.External.Test
{
    public sealed class ExternalLockablePropertyInterceptorAttribute : Attribute, ILockablePropertySetterInterceptor, ILockablePropertyGetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(SemaphoreSlim semaphore, PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }

        public bool OnSet(SemaphoreSlim semaphore, PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            semaphore.Wait();

            return false;
        }
    }
}