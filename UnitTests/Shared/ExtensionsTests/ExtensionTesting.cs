using Cauldron;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ExtensionsTests
{
    [TestClass]
    public class ExtensionTesting
    {
        [TestMethod]
        public void Concat_An_Item_To_Array_Test()
        {
            var a = new int[] { 34, 9, 233, 43 };
            var result = a.Concat(33);

            Assert.AreEqual(4, a.Length);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(33, result[4]);
            Assert.AreEqual(233, result[2]);
        }

        [TestMethod]
        public void Concat_Items_To_An_Array_Test()
        {
            var a = new int[] { 34, 9, 233, 43 };
            var b = new int[] { 22, 3 };
            var result = a.Concat(b);

            Assert.AreEqual(4, a.Length);
            Assert.AreEqual(2, b.Length);
            Assert.AreEqual(6, result.Length);
            Assert.AreEqual(22, result[4]);
            Assert.AreEqual(3, result[5]);
            Assert.AreEqual(233, result[2]);
            Assert.AreEqual(9, result[1]);
        }

        [TestMethod]
        public void Copy_An_Array()
        {
            var a = new int[] { 34, 9, 233, 43 };
            var b = a.Copy();

            Assert.IsTrue(a.SequenceEqual(b));
            Assert.IsFalse(object.ReferenceEquals(a, b));
        }

        [TestMethod]
        public void Modify_A_String_Without_Creating_A_New_Instance()
        {
            var myString = "Hello People";
            myString.ReplaceMe(new char[] { 'o' }, '*');

            Assert.AreEqual("Hell* Pe*ple", myString);
        }

        [TestMethod]
        public void Split_String_To_Multiple_Lines()
        {
            var testString = "Hello\r\nThis is a\nTest String\rMuhahah";
            var result = testString.GetLines();

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("Test String", result[2]);
        }
    }
}