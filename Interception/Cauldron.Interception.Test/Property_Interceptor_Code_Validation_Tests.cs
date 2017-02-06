using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Property_Interceptor_Code_Validation_Tests
    {
        private string k_BackingField;

        private int k_BackingField2;

        [TestPropertyInterceptor]
        public string TestProperty
        {
            get
            {
                TestPropertyInterceptorAttribute interceptor = new TestPropertyInterceptorAttribute();
                interceptor.OnGet(typeof(Property_Interceptor_Code_Validation_Tests), this, "TestProperty", null, k_BackingField, TestPropertySetter);

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
                TestPropertyInterceptorAttribute interceptor = new TestPropertyInterceptorAttribute();
                interceptor.OnGet(typeof(Property_Interceptor_Code_Validation_Tests), this, "TestProperty", null, k_BackingField2, TestPropertySetter2);

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
            this.k_BackingField2 = (int)value;
        }
    }
}