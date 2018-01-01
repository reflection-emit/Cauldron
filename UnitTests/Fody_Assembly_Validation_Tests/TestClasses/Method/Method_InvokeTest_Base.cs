using Cauldron.UnitTest.Interceptors;
using Cauldron.UnitTest.Interceptors.Method;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cauldron.UnitTest.AssemblyValidation.TestClasses.Method
{
    public class Method_InvokeTest_Base : IMethod_Interceptor_Invoke
    {
        public bool OnEnterInvoked { get; set; }
        public bool OnExceptionInvoked { get; set; }
        public bool OnExitInvoked { get; set; }

        [TestCleanup]
        public void CleanUp()
        {
            this.OnExceptionInvoked = false;
            this.OnEnterInvoked = false;
            this.OnExitInvoked = false;
        }

        public void OnEnter(string name)
        {
            this.OnEnterInvoked = true;
        }

        public void OnException(Exception e)
        {
            this.OnExceptionInvoked = true;
        }

        public void OnExit()
        {
            this.OnExitInvoked = true;
        }

        protected void AssertEnter()
        {
            Assert.IsTrue(this.OnEnterInvoked, "on enter was not invoked");
            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnExceptionInvoked, "The exception was invoked");
        }

        protected void AssertException(Action action)
        {
            try
            {
                action();
            }
            catch (InvokedException)
            {
                Assert.IsTrue(this.OnExceptionInvoked, "The exception was not invoked");
            }

            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnEnterInvoked, "onenter was invoked");
        }
    }
}