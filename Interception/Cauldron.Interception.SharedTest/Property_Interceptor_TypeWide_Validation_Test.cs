using Cauldron.Interception.External.Test;
using Cauldron.Interception.Test.Interceptors;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Property_Interceptor_TypeWide_Validation_Test
    {
        [TestMethod]
        public void InterceptAllProperty_Test()
        {
            var testClass = new TypeWidePropertyTestClass();

            testClass.Property1 = 66;
            testClass.Property2 = 66;

            Assert.AreEqual(99L, testClass.Property1);
            Assert.AreNotEqual(99L, testClass.Property2);
        }

        [TestPropertyInterceptor]
        public class TypeWidePropertyTestClass
        {
            public long Property1 { get; set; }

            [DoNotIntercept]
            public long Property2 { get; set; }

            public long Property3 { get; set; }
            public long Property4 { get; set; }
        }
    }
}