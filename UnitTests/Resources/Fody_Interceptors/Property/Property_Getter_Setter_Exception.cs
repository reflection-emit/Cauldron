using Cauldron.Interception;
using System;

namespace Cauldron.UnitTest.Interceptors.Property
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class Property_Getter_Setter_Exception : Attribute, IPropertyInterceptor
    {
        private IProperty_Interceptor_Invoke instance;

        public void OnException(Exception e)
        {
            this.instance?.OnException(e);
        }

        public void OnExit()
        {
            this.instance?.OnExit();
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            this.instance = propertyInterceptionInfo.Instance as IProperty_Interceptor_Invoke;
            throw new InvokedException();
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.instance = propertyInterceptionInfo.Instance as IProperty_Interceptor_Invoke;
            throw new InvokedException();
        }
    }
}