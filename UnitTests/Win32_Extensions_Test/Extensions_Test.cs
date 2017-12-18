using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cauldron;

namespace Win32_Extensions_Test
{
    [TestClass]
    public class Extensions_Test
    {
        [TestMethod]
        public void PadOrCut_CutBoth_Even_Test()
        {
            var str = "Lemurs".PadOrCut(4, Position.Both, '-');
            Assert.AreEqual("emur", str);
        }

        [TestMethod]
        public void PadOrCut_CutBoth_Uneven_Test()
        {
            var str = "Hello".PadOrCut(4, Position.Both, '-');
            Assert.AreEqual("Hell", str);
        }

        [TestMethod]
        public void PadOrCut_CutLeft_Test()
        {
            var str = "Hello".PadOrCut(4, Position.Left, '-');
            Assert.AreEqual("ello", str);
        }

        [TestMethod]
        public void PadOrCut_CutRight_Test()
        {
            var str = "Hello".PadOrCut(4, Position.Right, '-');
            Assert.AreEqual("Hell", str);
        }

        [TestMethod]
        public void PadOrCut_PadBoth_Even_Test()
        {
            var str = "Hell".PadOrCut(8, Position.Both, '-');
            Assert.AreEqual("--Hell--", str);
        }

        [TestMethod]
        public void PadOrCut_PadBoth_Uneven_Test()
        {
            var str = "Hello".PadOrCut(8, Position.Both, '-');
            Assert.AreEqual("-Hello--", str);
        }

        [TestMethod]
        public void PadOrCut_PadLeft_Test()
        {
            var str = "Hello".PadOrCut(8, Position.Left, '-');
            Assert.AreEqual("---Hello", str);
        }

        [TestMethod]
        public void PadOrCut_PadRight_Test()
        {
            var str = "Hello".PadOrCut(8, Position.Right, '-');
            Assert.AreEqual("Hello---", str);
        }

        [TestMethod]
        public void PadOrCut_SameLength_Test()
        {
            var str1 = "Hello";
            var str2 = str1.PadOrCut(5, Position.Right, '-');
            Assert.IsTrue(object.ReferenceEquals(str1, str2));
            Assert.AreEqual(str1, str2);
        }
    }
}