using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win32_Fody_AssemblyWide_Validator_Tests
{
    [TestClass]
    public class InterceptorAdditionValidatation
    {
        public DateTime Bla { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
        }

        private void Muhahaha(object bla)
        {
        }
    }
}