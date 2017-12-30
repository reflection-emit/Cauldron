using Cauldron.UnitTest.Interceptors;
using Cauldron.UnitTest.Interceptors.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Cauldron.UnitTest.AssemblyValidation.TestClasses.Property
{
    public class Property_InvokeTest_Base : IProperty_Interceptor_Invoke
    {
        public bool OnExceptionInvoked { get; set; }
        public bool OnExitInvoked { get; set; }
        public bool OnGetInvoked { get; set; }
        public bool OnSetInvoked { get; set; }

        [TestCleanup]
        public void CleanUp()
        {
            this.OnExceptionInvoked = false;
            this.OnGetInvoked = false;
            this.OnSetInvoked = false;
            this.OnExitInvoked = false;
        }

        public void OnException(Exception e)
        {
            this.OnExceptionInvoked = true;
        }

        public void OnExit()
        {
            this.OnExitInvoked = true;
        }

        public void OnGet(string name, object value)
        {
            this.OnGetInvoked = true;
        }

        public void OnSet(string name, object oldValue, object newValue)
        {
            this.OnSetInvoked = true;
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
            Assert.IsFalse(this.OnGetInvoked, "onget was invoked");
            Assert.IsFalse(this.OnSetInvoked, "onset was invoked");
        }

        protected void AssertException<T>(Func<T> action)
        {
            try
            {
                T value = action();
            }
            catch (InvokedException)
            {
                Assert.IsTrue(this.OnExceptionInvoked, "The exception was not invoked");
            }

            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnGetInvoked, "onget was invoked");
            Assert.IsFalse(this.OnSetInvoked, "onset was invoked");
        }

        protected void AssertGetter()
        {
            Assert.IsTrue(this.OnGetInvoked, "on get was not invoked");
            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnExceptionInvoked, "The exception was invoked");
            Assert.IsFalse(this.OnSetInvoked, "onset was invoked");
        }

        protected void AssertNoSetter<T>(Func<T> propertyGet, Action<T> propertySet, T value)
        {
            var oldValue = propertyGet();
            propertySet(value);

            Assert.IsTrue(this.OnGetInvoked, "on get was not invoked");
            Assert.IsTrue(this.OnSetInvoked, "on get was not invoked");
            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnExceptionInvoked, "The exception was invoked");

            Assert.AreNotEqual(oldValue, value);
        }

        protected void AssertSetter()
        {
            Assert.IsTrue(this.OnSetInvoked, "on get was not invoked");
            Assert.IsTrue(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnExceptionInvoked, "The exception was invoked");
            Assert.IsFalse(this.OnGetInvoked, "onget was invoked");
        }
    }
}