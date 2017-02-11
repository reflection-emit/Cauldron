using Cauldron.Interception.External.Test;
using Cauldron.Interception.Test.Interceptors;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Property_Interceptor_Code_Validation_Tests
    {
        [ExternalLockablePropertyInterceptor]
        public static string StaticLockableProperty { get; set; }

        [TestPropertyInterceptor]
        public static double StaticProperty { get; set; }

        [TestPropertyInterceptor]
        public long[] ArrayProperty { get; set; }

        [TestPropertyInterceptor]
        public ITestInterface InterfaceProperty { get; set; }

        [TestPropertyInterceptor]
        public List<long> ListProperty { get; set; }

        [ExternalLockablePropertyInterceptor]
        public string LockableProperty { get; set; }

        [EnumPropertyInterceptor]
        public TestEnum PropertyWithEnumValue { get; private set; }

        [TestPropertyInterceptor]
        public long ValueTypeProperty { get; set; }

        [TestPropertyInterceptor]
        public long ValueTypePropertyPrivateSetter { get; private set; }

        [TestMethod]
        public void Array_Property_Setter()
        {
            this.ArrayProperty = new long[0];

            Assert.AreEqual(4, this.ArrayProperty.Length);
            Assert.AreEqual(5643, this.ArrayProperty[2]);
        }

        [TestMethod]
        public void EnumProperty_Property_Getter()
        {
            this.PropertyWithEnumValue = (TestEnum)20;
            Assert.AreEqual((TestEnum)45, this.PropertyWithEnumValue);

            this.PropertyWithEnumValue = (TestEnum)5;
            Assert.AreEqual(TestEnum.Two, this.PropertyWithEnumValue);

            this.PropertyWithEnumValue = (TestEnum)12;
            Assert.AreEqual((TestEnum)232, this.PropertyWithEnumValue);
        }

        [TestMethod]
        public void EnumProperty_Property_Setter()
        {
            try
            {
                this.PropertyWithEnumValue = TestEnum.Three;
                this.PropertyWithEnumValue = TestEnum.Three;

                Assert.IsTrue(false);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void List_Property_Setter()
        {
            this.ListProperty = new List<long>();

            Assert.AreEqual(4, this.ListProperty.Count);
            Assert.AreEqual(5643, this.ListProperty[2]);
        }

        [TestMethod]
        public void LockableProperties()
        {
            this.LockableProperty = "Hello";
            StaticLockableProperty = "Computer";
            Assert.AreEqual("Hello", this.LockableProperty);
            Assert.AreEqual("Computer", StaticLockableProperty);
        }

        [TestMethod]
        public void Static_Property()
        {
            StaticProperty = 4.6;
            Assert.AreEqual(4.6, StaticProperty);

            StaticProperty = 66;
            Assert.AreEqual(78344.796875, StaticProperty);
        }

        [TestMethod]
        public void ValueType_Property()
        {
            this.ValueTypeProperty = 50;
            Assert.AreEqual(50, this.ValueTypeProperty);

            this.ValueTypeProperty = 30;
            Assert.AreEqual(9999, this.ValueTypeProperty);
        }
    }
}