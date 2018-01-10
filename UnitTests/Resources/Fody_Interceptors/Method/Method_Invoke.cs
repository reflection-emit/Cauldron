using Cauldron.Interception;
using Cauldron.UnitTest.Interceptors.Property.RuleAttribute;
using System;
using System.Reflection;

namespace Cauldron.UnitTest.Interceptors.Method
{
    [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(SuppressInterceptorAttribute))]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Method_Invoke : Attribute, IMethodInterceptor
    {
        private IMethod_Interceptor_Invoke instance;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            this.instance = instance as IMethod_Interceptor_Invoke;
            this.instance?.OnEnter(methodbase.Name);
        }

        public bool OnException(Exception e)
        {
            this.instance?.OnException(e);
            return true;
        }

        public void OnExit()
        {
            this.instance?.OnExit();
        }
    }
}