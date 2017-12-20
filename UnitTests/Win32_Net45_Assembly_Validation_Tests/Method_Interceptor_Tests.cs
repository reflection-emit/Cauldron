using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Win32_Net45_Assembly_Validation_Tests
{
    #region Interceptors

    public class MethodInterceptor_EntryAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values) => throw new MethodInterceptor_Exception();

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }

    public class MethodInterceptor_Exception : Exception
    {
    }

    public class MethodInterceptor_ExceptionAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public void OnException(Exception e) => throw new MethodInterceptor_Exception();

        public void OnExit()
        {
        }
    }

    public class MethodInterceptor_ExitAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit() => throw new MethodInterceptor_Exception();
    }

    #endregion Interceptors

    [TestClass]
    public class Method_Interceptor_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(MethodInterceptor_Exception))]
        public void Method_Interceptor_Invoke_OnEnter_Test() => this.EnterInvoke();

        [TestMethod]
        [ExpectedException(typeof(MethodInterceptor_Exception))]
        public void Method_Interceptor_Invoke_OnException_Test() => this.ExitInvoke();

        [TestMethod]
        [ExpectedException(typeof(MethodInterceptor_Exception))]
        public void Method_Interceptor_Invoke_OnExit_Test() => this.ExitInvoke();

        #region Intercepted Methods

        [MethodInterceptor_Entry]
        private void EnterInvoke()
        {
        }

        [MethodInterceptor_Exception]
        private void ExceptionInvoke()
        {
        }

        [MethodInterceptor_Exit]
        private void ExitInvoke()
        {
        }

        #endregion Intercepted Methods
    }
}