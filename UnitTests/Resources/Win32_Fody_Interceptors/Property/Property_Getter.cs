using Cauldron.Interception;
using System;

namespace Cauldron.UnitTest.Interceptors.Property
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class Property_Getter : Attribute, IPropertyGetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }
    }
}