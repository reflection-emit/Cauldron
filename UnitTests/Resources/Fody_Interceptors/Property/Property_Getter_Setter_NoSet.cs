using Cauldron.Interception;
using System;

namespace Cauldron.UnitTest.Interceptors.Property
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class Property_Getter_Setter_NoSet : Attribute, IPropertyInterceptor
    {
        private IProperty_Interceptor_Invoke instance;

        public bool OnException(Exception e)
        {
            this.instance?.OnException(e);
            return true;
        }

        public void OnExit()
        {
            this.instance?.OnExit();
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            this.instance = propertyInterceptionInfo.Instance as IProperty_Interceptor_Invoke;
            this.instance?.OnGet(propertyInterceptionInfo.PropertyName, value);
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.instance = propertyInterceptionInfo.Instance as IProperty_Interceptor_Invoke;
            this.instance?.OnSet(propertyInterceptionInfo.PropertyName, oldValue, newValue);
            return true;
        }
    }
}