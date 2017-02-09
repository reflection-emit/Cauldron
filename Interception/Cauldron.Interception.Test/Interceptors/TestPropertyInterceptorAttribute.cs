using Cauldron.Core;
using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TestPropertyInterceptorAttribute : Attribute, IPropertyGetterInterceptor, IPropertySetterInterceptor
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
                propertyInterceptionInfo.SetValue(9999);

            if (ComparerUtils.Equals(value, (double)66))
                propertyInterceptionInfo.SetValue(78344.8f);
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            if (propertyInterceptionInfo.PropertyType.IsArray)
            {
                var passed = new List<long>();
                passed.Add(2);
                passed.Add(232);
                passed.Add(5643);
                passed.Add(52435);
                propertyInterceptionInfo.SetValue(passed);
                return true;
            }

            return false;
        }
    }
}