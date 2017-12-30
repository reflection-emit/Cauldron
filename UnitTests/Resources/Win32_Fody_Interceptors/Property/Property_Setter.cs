using Cauldron.Interception;
using System;

namespace Cauldron.UnitTest.Interceptors.Property
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class Property_Setter : Attribute, IPropertySetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            return false;
        }
    }
}