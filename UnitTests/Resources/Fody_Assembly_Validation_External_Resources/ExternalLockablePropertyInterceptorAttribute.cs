using Cauldron.Interception;
using System;

namespace Fody_Assembly_Validation_External_Resources
{
    public sealed class ExternalLockablePropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor, ISyncRoot
    {
        public object SyncRoot { get; set; }

        public bool OnException(Exception e) => true;

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