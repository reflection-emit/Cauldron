using Cauldron.Core.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace UnitTest_ReflectionTests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void EntryAssembly_PreUWP16299_Hack_Test()
        {
            Assert.AreEqual(typeof(ReflectionTests).GetTypeInfo().Assembly, Assemblies.EntryAssembly);
        }

        [TestMethod]
        public void ReferencedAssemblies_UWP_Hack()
        {
            Assert.IsTrue(Assemblies.Known.Length > 10);
        }
    }
}