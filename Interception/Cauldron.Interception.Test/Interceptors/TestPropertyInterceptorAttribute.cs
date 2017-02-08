using Cauldron.Core;
using Cauldron.Core.Interceptors;
using System;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TestPropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (ComparerUtils.Equals(value, (long)30))
                propertyInterceptionInfo.SetValue(9999);

            if (ComparerUtils.Equals(value, (double)66))
                propertyInterceptionInfo.SetValue(78344.8f);
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            return false;
        }
    }
}