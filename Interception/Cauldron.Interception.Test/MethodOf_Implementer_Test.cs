using Cauldron.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class MethodOf_Implementer_Test
    {
        private MethodBase testField;

        public MethodBase TestProperty
        {
            get
            {
                return Reflection.GetMethodBase();
            }
        }

        [TestMethod]
        public void MethodOf_Method()
        {
            var methodBase = Reflection.GetMethodBase();

            Assert.AreNotEqual(null, methodBase);
            Assert.AreEqual("MethodOf_Method", methodBase.Name);
        }

        [TestMethod]
        public void MethodOf_Method_To_Field()
        {
            testField = Reflection.GetMethodBase();

            Assert.AreNotEqual(null, testField);
            Assert.AreEqual("MethodOf_Method_To_Field", testField.Name);
        }

        [TestMethod]
        public void MethodOf_Property_Getter()
        {
            var propertyGetter = this.TestProperty;

            Assert.AreNotEqual(null, propertyGetter);
            Assert.AreEqual("get_TestProperty", propertyGetter.Name);
        }
    }
}