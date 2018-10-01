using Cauldron.Interception;
using System;
using System.Reflection;

namespace UnitTests.BasicInterceptors
{
    public class ParameterPassedInterceptor : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            if (
                values.Length == 3 &&
                values[0] is int v1 &&
                values[1] is string v2 &&
                values[2] is bool v3 &&
                v1 == 2 &&
                v2 == "Hello" &&
                v3 == true)
                throw new ParameterPassedInterceptorException();
        }

        public bool OnException(Exception e)
        {
            return true;
        }

        public void OnExit()
        {
        }
    }

    public class ParameterPassedInterceptorException : Exception
    {
        public ParameterPassedInterceptorException() : base()
        {
        }
    }
}