using Cauldron.Interception.Test.Interceptors;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using System;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Method_Interceptor_TypeWide_Validation_Test
    {
#if !WINDOWS_UWP

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InterceptAllMethods_Test()
        {
            var testClass = new TypeWideMethodTestClass();
            testClass.Method1();
        }

#else
        [TestMethod]
        public void InterceptAllMethods_Test()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var testClass = new TypeWideMethodTestClass();
                testClass.Method1();
            });
        }
#endif

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