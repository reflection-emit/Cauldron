using Cauldron.Interception;
using System;
using System.Reflection;

namespace UnitTest_InterceptorsForTest
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LogEnterExitAttribute : Attribute, IMethodInterceptor, IPropertyInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }

        bool IMethodInterceptor.OnException(Exception e)
        {
            throw new NotImplementedException();
        }

        public bool OnException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void OnExit()
        {
            throw new NotImplementedException();
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            throw new NotImplementedException();
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}