using Cauldron.Interception;
using System;
using System.Reflection;

namespace UnitTest_InterceptorsForTest
{
    public class SimpleInterceptorAttribute : Attribute, ISimpleMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }
    }

    [InterceptorOptions(AlwaysCreateNewInstance = true)]
    public class SimpleInterceptorWithoutInstanceAttribute : Attribute, ISimpleMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }
    }

    [InterceptorOptions(AlwaysCreateNewInstance = true)]
    public class SimpleInterceptorWithSyncAndInstanceAttribute : Attribute, ISimpleMethodInterceptor, ISyncRoot
    {
        public object SyncRoot { get; set; }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleInterceptorWithSyncAttribute : Attribute, ISimpleMethodInterceptor, ISyncRoot
    {
        public object SyncRoot { get; set; }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }
    }
}