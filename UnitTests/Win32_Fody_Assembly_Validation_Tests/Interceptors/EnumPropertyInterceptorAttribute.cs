using Cauldron.Interception;
using System;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EnumPropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor
    {
        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (Convert.ToInt32(value) == 20)
                propertyInterceptionInfo.SetValue("45");

            if (Convert.ToInt32(value) == 5)
                propertyInterceptionInfo.SetValue(TestEnum.Two);

            if (Convert.ToInt32(value) == 12)
                propertyInterceptionInfo.SetValue(232);
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            if (oldValue == newValue)
                throw new Exception("");

            return false;
        }
    }
}