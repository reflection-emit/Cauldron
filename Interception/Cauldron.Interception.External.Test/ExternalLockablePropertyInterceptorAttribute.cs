using System;
using System.Threading;

namespace Cauldron.Interception.External.Test
{
    public sealed class ExternalLockablePropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor
    {
        private object lockObject = new object();

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                lock (lockObject)
                {
                }
            }

            return false;
        }
    }
}