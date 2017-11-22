using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win32_Fody_Assembly_Validation_Tests
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