using Cauldron.Interception;
using System;

namespace Win32_Fody_Assembly_Validation_Tests
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
                propertyInterceptionInfo.SetValue(new TestClass());
        }
    }
}