using Cauldron.Core.Interceptors;
using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Property_Interceptor_Code_Validation_Tests
    {
        private MethodBase huhu;
        private string k_BackingField;

        private int k_BackingField2;

        private PropertyInterceptionInfo propertyInterceptionInfo;
        private PropertyInterceptionInfo propertyInterceptionInfo2;

        [TestPropertyInterceptor]
        public ITestInterface InterfaceProperty { get; set; }

        public string TestProperty
        {
            get
            {
                if (propertyInterceptionInfo == null)
                    propertyInterceptionInfo = new PropertyInterceptionInfo(huhu, "TestProperty", typeof(string), this, TestPropertySetter);

                TestPropertyInterceptorAttribute interceptor = new TestPropertyInterceptorAttribute();
                interceptor.OnGet(propertyInterceptionInfo, k_BackingField);

                return k_BackingField;
            }
            set
            {
                k_BackingField = value;
            }
        }

        public int TestProperty2
        {
            get
            {
                if (propertyInterceptionInfo2 == null)
                    propertyInterceptionInfo2 = new PropertyInterceptionInfo(null, "TestProperty2", typeof(int), this, TestPropertySetter2);

                TestPropertyInterceptorAttribute interceptor = new TestPropertyInterceptorAttribute();
                interceptor.OnGet(propertyInterceptionInfo2, k_BackingField2);

                return k_BackingField2;
            }
            set
            {
                k_BackingField2 = value;
            }
        }

        [TestPropertyInterceptor]
        public long ValueTypeProperty { get; set; }

        [TestMethod]
        public void ValueType_Property()
        {
            this.ValueTypeProperty = 50;
            Assert.AreEqual(50, this.ValueTypeProperty);

            this.ValueTypeProperty = 30;
            Assert.AreEqual(9999, this.ValueTypeProperty);
        }

        private void TestPropertySetter(object value)
        {
            this.k_BackingField = (string)value;
        }

        private void TestPropertySetter2(object value)
        {
            ITestInterface zz = value as ITestInterface;

            this.k_BackingField2 = Convert.ToInt32(value);
        }
    }
}