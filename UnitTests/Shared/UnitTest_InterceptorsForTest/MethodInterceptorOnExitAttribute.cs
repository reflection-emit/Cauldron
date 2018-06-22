using Cauldron.Interception;
using System;
using System.Reflection;

namespace UnitTest_InterceptorsForTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class MethodInterceptorOnExitAttribute : Attribute, IMethodInterceptor, IMethodInterceptorOnExit
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e)
        {
            return true;
        }

        public object OnExit(Type returnType, object returnValue)
        {
            if (returnValue is float floatValue && floatValue == 0.9f)
                return 22.99f;

            if (typeof(double) == returnType)
                return 3.4;

            if (typeof(string) == returnType)
                return "Hello";

            if (typeof(int) == returnType)
                return 35;

            if (typeof(void) == returnType)
                throw new NotImplementedException();

            return returnValue;
        }

        public void OnExit()
        {
        }
    }
}