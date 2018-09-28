using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace UnitTests.Reflection
{
    public class Module
    {
        public static void ModuleLoad(Assembly[] referenceCopyLocal)
        {
            ModuleLoadTests.triggered = true;
        }
    }

    [TestClass]
    public class ModuleLoadTests
    {
        internal static bool triggered = false;

        [TestMethod]
        public void Is_ModuleLoad_Weaved()
        {
            Assert.AreEqual(true, triggered);
        }
    }
}