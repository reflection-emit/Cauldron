using Cauldron.Test.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace Cauldron.Test
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void Constructor_Injection_Test()
        {
        }

        [TestMethod]
        public void CreateMany_Test()
        {
            var animals = Factory.CreateMany<IAnimal>();
            Assert.AreEqual(4, animals.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(AmbiguousMatchException))]
        public void CreateSingle_With_AmbiguousMatchException_Test()
        {
            var animals = Factory.Create<IAnimal>();
        }

        [TestMethod]
        public void CreateSingleType_Test()
        {
            var cat = Factory.Create<Cat>();
            Assert.AreEqual("Cat", cat.Name);
        }
    }
}