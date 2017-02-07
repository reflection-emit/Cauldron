using Cauldron.Core;
using Cauldron.Core.Interceptors;
using System;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TestPropertyInterceptorAttribute : Attribute, IPropertyInterceptor
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
                propertyInterceptionInfo.Set(9999);
        }
    }
}