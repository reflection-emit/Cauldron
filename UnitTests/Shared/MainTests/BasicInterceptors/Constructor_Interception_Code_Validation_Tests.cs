using MainTests.BasicInterceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests.BasicInterceptors
{
    [TestClass]
    public class Constructor_Interception_Code_Validation_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Instantiate_Class_Test()
        {
            var newObject = new Ctor_TestClass1();
            Assert.IsTrue(true);
        }
    }
}