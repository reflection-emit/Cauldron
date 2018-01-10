using Cauldron.Interception;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.UnitTest.AssemblyValidation
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class Exception_Class_Interceptor : Attribute, IMethodInterceptor
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
        public Task Task { get; set; }

        [Exception_Class_Interceptor2]
        public Task<int> Task2 { get; set; }

        public Task<T> Bla<T>()
        {
            return Task.FromResult(default(T));
        }

        public Task<int> Bla()
        {
            return Task.FromResult(0);
        }

        public Task BlaBla()
        {
            return Task.FromResult(0);
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