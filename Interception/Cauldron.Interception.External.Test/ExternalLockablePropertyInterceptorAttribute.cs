using System;

namespace Cauldron.Interception.External.Test
{
    public sealed class ExternalLockablePropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor, ISyncRoot
    {
        public object SyncRoot { get; set; }

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
            if (SyncRoot == null)
                throw new ArgumentNullException(nameof(SyncRoot));

            if (newValue != oldValue)
            {
                lock (SyncRoot)
                {
                }
            }

            return false;
        }
    }
}