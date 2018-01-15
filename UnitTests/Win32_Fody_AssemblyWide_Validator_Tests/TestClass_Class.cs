using Cauldron.Interception;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.UnitTest.AssemblyValidation
{
    public struct TestStruct
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class Exception_Class_Interceptor : Attribute, IMethodInterceptor, IPropertyInterceptor, IPropertyInterceptorInitialize
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e)
        {
            return true;
        }

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            throw new NotImplementedException();
        }

        public void OnInitialize(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            throw new NotImplementedException();
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class Exception_Class_Interceptor2 : Attribute, ISimpleMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }
    }

    public class TestClass_One_Class
    {
        [Exception_Class_Interceptor]
        public Task Task { get; set; }

        [Exception_Class_Interceptor]
        public Task<int> Task2 { get; set; }

        public Task<T> Bla<T>()
        {
            return Task.FromResult(default(T));
        }

        public Task<int> Bla()
        {
            return Task.FromResult(0);
        }

        public TestStruct Blaaa()
        {
            return default(TestStruct);
        }

        public Task BlaBla()
        {
            return Task.FromResult(0);
        }

        public Task<T> Blub<T>() where T : new()
        {
            return Task.FromResult(default(T));
        }
    }

    internal class TestClass_Two_Class
    {
        public T Test<T>()
        {
            return default(T);
        }
    }
}