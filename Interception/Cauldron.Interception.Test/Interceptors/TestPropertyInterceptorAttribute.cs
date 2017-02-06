using Cauldron.Core;
using Cauldron.Core.Interceptors;
using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TestPropertyInterceptorAttribute : Attribute, IPropertyInterceptor
    {
        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public void OnGet(Type declaringType, object instance, string propertyName, MethodBase methodbase, object value, Action<object> setter)
        {
            if (ComparerUtils.Equals(value, (long)30))
                setter((long)9999);
        }
    }
}