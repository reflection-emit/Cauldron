using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
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
            if (propertyInterceptionInfo.PropertyType.IsArray || propertyInterceptionInfo.PropertyType.AreReferenceAssignable(typeof(List<long>)))
            {
                var passed = new List<long>();
                passed.Add(2);
                passed.Add(232);
                passed.Add(5643);
                passed.Add(52435);
                propertyInterceptionInfo.SetValue(passed);
                return true;
            }

            if (propertyInterceptionInfo.PropertyType == typeof(long) && Convert.ToInt64(newValue) == 66L)
            {
                propertyInterceptionInfo.SetValue(99L);
                return true;
            }

            return false;
        }
    }
}