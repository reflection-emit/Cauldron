using System;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CreateATypeInterceptorAttribute : Attribute, IPropertyGetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (value == null)
                propertyInterceptionInfo.SetValue(new TestClass());
        }
    }
}