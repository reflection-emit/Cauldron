using Cauldron.Interception;
using System;

namespace UnitTest_InterceptorsForTest
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CreateATypeInterceptorAttribute : Attribute, IPropertyGetterInterceptor
    {
        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (value == null)
                propertyInterceptionInfo.SetValue(Activator.CreateInstance(propertyInterceptionInfo.PropertyType));
        }
    }
}