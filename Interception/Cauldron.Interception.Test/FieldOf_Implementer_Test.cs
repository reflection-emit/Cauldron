using Cauldron.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class FieldOf_Implementer_Test
    {
        private static string field2 = "hi";
        private string field1 = "hi";

        [TestMethod]
        public void Instanced_Private_Field_Info()
        {
            var field = Reflection.GetFieldInfo(nameof(field1));

            Assert.AreNotEqual(null, field);
            Assert.AreEqual("field1", field.Name);
        }

        [TestMethod]
        public void Static_Private_Field_From_Other_Class_Info()
        {
            var field = Reflection.GetFieldInfo("Cauldron.Interception.Test.OtherClass.aprivateField");

            Assert.AreNotEqual(null, field);
            Assert.AreEqual("aprivateField", field.Name);
        }

        [TestMethod]
        public void Static_Private_Field_Info()
        {
            var field = Reflection.GetFieldInfo(nameof(field2));

            Assert.AreNotEqual(null, field);
            Assert.AreEqual("field2", field.Name);
        }

        [TestMethod]
        public void Static_Public_Field_From_Other_Class_Info()
        {
            var field = Reflection.GetFieldInfo("Cauldron.Interception.Test.OtherClass.apublicField");

            Assert.AreNotEqual(null, field);
            Assert.AreEqual("apublicField", field.Name);
        }
    }

    public class OtherClass
    {
        public static int apublicField = 99;
        private static double aprivateField = 33.5;
    }
}