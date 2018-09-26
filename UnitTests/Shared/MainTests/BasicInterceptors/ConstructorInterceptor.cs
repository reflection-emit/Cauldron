using Cauldron.Interception;
using System;
using System.Reflection;

namespace MainTests.BasicInterceptors
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ConstructorInterceptor : Attribute, IConstructorInterceptor
    {
        public void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values)
        {
            throw new NotImplementedException();
        }

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
    }
}