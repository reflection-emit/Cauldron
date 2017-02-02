using Cauldron.Core.Interceptors;
using System;

namespace Cauldron.Interception.Test.Interceptors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TestMethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter()
        {
            throw new NotImplementedException();
        }

        public void OnException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void OnExit()
        {
            throw new NotImplementedException();
        }
    }
}