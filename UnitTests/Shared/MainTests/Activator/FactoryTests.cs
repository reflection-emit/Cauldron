using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace UnitTests.Activator
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void Class_With_Enum_In_Ctor_Test()
        {
            var instance = Factory.Create<ClassWithEnumInCtor>(AnyEnumWillDo.Two);

            Assert.AreEqual(AnyEnumWillDo.Two, instance.TheEnum);
        }

        [TestMethod]
        public void Create_By_Static_Property()
        {
            var instance = Factory.Create<ConstructionByProperty>();

            Assert.AreEqual(300, instance.Value);
        }

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

        [ExpectedException(typeof(AmbiguousMatchException))]
        [TestMethod]
        public void CreateSingle_With_AmbiguousMatchException_Test()
        {
            Factory.Create<IAnimal>();
        }

        [TestMethod]
        public void CreateSingleType_Test()
        {
            var cat = Factory.Create<Cat>();
            Assert.AreEqual("Cat", cat.Name);
        }
    }

    #region Resources

    public enum AnyEnumWillDo
    {
        One,
        Two
    }

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

    [Component(typeof(ClassWithEnumInCtor))]
    public class ClassWithEnumInCtor
    {
        [ComponentConstructor]
        public ClassWithEnumInCtor(AnyEnumWillDo @enum)
        {
            this.TheEnum = @enum;
        }

        public AnyEnumWillDo TheEnum { get; private set; }
    }

    [Component(typeof(ConstructionByProperty))]
    public class ConstructionByProperty
    {
        [ComponentConstructor]
        public static ConstructionByProperty Current
        {
            get { return new ConstructionByProperty { Value = 300 }; }
        }

        public int Value { get; set; }
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