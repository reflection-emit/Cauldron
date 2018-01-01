using Cauldron.UnitTest.AssemblyValidation.TestClasses.Method;
using Cauldron.UnitTest.Interceptors.Method;
using Cauldron.UnitTest.Interceptors.Property.RuleAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.UnitTest.AssemblyValidation.TestClasses.InterceptionRuleAttribute
{
    [TestClass]
    public sealed class Method_Interceptor_Suppress_Tests : Method_InvokeTest_Base
    {
        [TestMethod]
        public void Method_With_Interceptor_But_Suppress()
        {
            __Method_With_Interceptor_But_Suppress();

            Assert.IsFalse(this.OnEnterInvoked, "on enter was not invoked");
            Assert.IsFalse(this.OnExitInvoked, "The exit was not invoked");
            Assert.IsFalse(this.OnExceptionInvoked, "The exception was invoked");
        }

        [Method_Invoke]
        [SuppressInterceptor]
        private void __Method_With_Interceptor_But_Suppress()
        {
        }
    }
}