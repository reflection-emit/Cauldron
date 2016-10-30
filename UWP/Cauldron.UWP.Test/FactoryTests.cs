using Cauldron.Activator;
using System;
using System.Linq;
using System.Reflection;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Test
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void Create_Type_With_Private_Constructor()
        {
            var obj = Factory.Create<TypeWithPrivateConstructor>();
            Assert.AreNotEqual(null, obj);
        }

        [TestMethod]
        public void CreateMany_Test()
        {
            var animals = Factory.CreateMany<IAnimal>();
            Assert.AreEqual(5, animals.Count());
        }

#if !WINDOWS_UWP

        [ExpectedException(typeof(AmbiguousMatchException))]
#endif
        [TestMethod]
        public void CreateSingle_With_AmbiguousMatchException_Test()
        {
#if WINDOWS_UWP
            Assert.ThrowsException<AmbiguousMatchException>(() => Factory.Create<IAnimal>());
#else
            Factory.Create<IAnimal>();
#endif
        }

        [TestMethod]
        public void CreateSingleType_Test()
        {
            var cat = Factory.Create<Cat>();
            Assert.AreEqual("Cat", cat.Name);
        }
    }

    #region Resources

    public interface IAnimal
    {
        string Name { get; }
    }

    public abstract class Bird : IAnimal
    {
        public abstract string Name { get; }
    }

    public class Cat : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Component(typeof(IAnimal))]
    public class Dog : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Component(typeof(IAnimal))]
    public class Elephant : IAnimal
    {
        private Elephant()
        {
        }

        public string Name { get { return this.GetType().Name; } }

        [ComponentConstructor]
        public static Elephant UseThisToCreate()
        {
            return new Elephant();
        }
    }

    [Component(typeof(IAnimal))]
    public class Leopard : IAnimal
    {
        public string Name { get { return this.GetType().Name; } }
    }

    [Component(typeof(IAnimal))]
    public class Parrot : Bird
    {
        public override string Name { get { return this.GetType().Name; } }
    }

    [Component(typeof(IAnimal))]
    public class Pigeon : Bird
    {
        public override string Name { get { return this.GetType().Name; } }
    }

    public class TypeWithPrivateConstructor
    {
        [ComponentConstructor]
        private TypeWithPrivateConstructor()
        {
        }
    }

    #endregion Resources
}