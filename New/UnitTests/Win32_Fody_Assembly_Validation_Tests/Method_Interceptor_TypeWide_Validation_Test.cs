using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [TestClass]
    public class Method_Interceptor_TypeWide_Validation_Test
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InterceptAllMethods_Test()
        {
            var testClass = new TypeWideMethodTestClass();
            testClass.Method1();
        }

        [TestMethod]
        public void Method_Interceptor_Respect_DoNotInterceptAttribute_Test()
        {
            var testClass = new TypeWideMethodTestClass();
            testClass.Method2();

            Assert.IsTrue(true);
        }

        [ExceptionThrowingMethodInterceptor]
        public class TypeWideMethodTestClass
        {
            public void Method1()
            {
            }

            [DoNotIntercept]
            public void Method2()
            {
            }
        }
    }
}