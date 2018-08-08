using Cauldron.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace UnitTests.Reflection
{
    [TestClass]
    public class ReflectionTests
    {
#if NETFX_CORE
        [TestMethod]
        public void EntryAssembly_PreUWP16299_Hack_Test()
        {
            Assert.AreEqual(typeof(ReflectionTests).GetTypeInfo().Assembly, Assemblies.EntryAssembly);
        }
#endif
        [TestMethod]
        public void ReferencedAssemblies_UWP_Hack()
        {
            Assert.IsTrue(Assemblies.Known.Length > 1);
            Assert.IsTrue(Assemblies.Known.Contains(typeof(ReflectionTests).GetTypeInfo().Assembly));
        }
    }
}