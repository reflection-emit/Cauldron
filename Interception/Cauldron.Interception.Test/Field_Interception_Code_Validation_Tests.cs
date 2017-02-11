using Cauldron.Interception.Test.Interceptors;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Field_Interception_Code_Validation_Tests
    {
        [TestPropertyInterceptor]
        private static double fieldTwo = 0.3;

        [TestPropertyInterceptor]
        private long fieldOne;

        [TestPropertyInterceptor]
        private int? nullableValueType;

        [TestMethod]
        public void Instance_Method_Field_Interception()
        {
            this.fieldOne = 50;
            Assert.AreEqual(50, this.fieldOne);

            this.fieldOne = 30;
            Assert.AreEqual(9999, this.fieldOne);
        }

        [TestMethod]
        public void Instance_Nullable_Method_Field_Interception()
        {
            this.nullableValueType = 50;
            Assert.AreEqual(50, this.nullableValueType);

            this.nullableValueType = 30;
            Assert.AreEqual(30, this.nullableValueType);
        }

        [TestMethod]
        public void Static_Method_Field_Interception()
        {
            fieldTwo = 4.6;
            Assert.AreEqual(4.6, fieldTwo);

            fieldTwo = 66;
            Assert.AreEqual(78344.796875, fieldTwo);
        }
    }
}