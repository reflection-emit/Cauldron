using Cauldron.Core;
using System;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    public class EnumPropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor
    {
        public void OnException(Exception e)
        {
        }

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
            if (ComparerUtils.Equals(oldValue, newValue))
                throw new Exception("");

            return false;
        }
    }
}