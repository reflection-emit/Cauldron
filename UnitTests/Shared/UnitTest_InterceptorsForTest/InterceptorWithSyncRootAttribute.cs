using Cauldron.Interception;
using System;
using System.Reflection;

namespace UnitTest_InterceptorsForTest
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InterceptorWithSyncRootAttribute : Attribute, IMethodInterceptor, IPropertyInterceptor, ISyncRoot
    {
        public object SyncRoot { get; set; }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
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