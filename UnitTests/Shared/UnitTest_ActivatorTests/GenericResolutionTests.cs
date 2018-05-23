using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[module: GenericComponent(typeof(Activator_Tests.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(Activator_Tests.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(Activator_Tests.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(Activator_Tests.GenericClassWithStaticCtor<Activator_Tests.TestClass>), typeof(Activator_Tests.IGenericClassWithStaticCtor<Activator_Tests.TestClass>))]

namespace Activator_Tests
{
    public interface IGenericClassWithStaticCtor<T>
    {
        T Item { get; }
    }

    public class ClassWithGeneric<T1, T2>
    {
        public ClassWithGeneric()
        {
        }

        public ClassWithGeneric(T1 a, T2 b)
        {
            this.Property1 = a;
            this.Property2 = b;
        }

        public T1 Property1 { get; set; }

        public T2 Property2 { get; set; }
    }

    public class GenericClassWithStaticCtor<T> : IGenericClassWithStaticCtor<T>
    {
        private GenericClassWithStaticCtor()
        {
        }

        [ComponentConstructor]
        public static GenericClassWithStaticCtor<T> Instance { get; } = new GenericClassWithStaticCtor<T>();

        public T Item { get; }
    }

    [TestClass]
    public class GenericResolutionTests
    {
        [TestMethod]
        public void Generic_Class_With_Static_Constructor()
        {
            var o = Factory.Create<IGenericClassWithStaticCtor<TestClass>>();
            o.Item.Height = 20.0;

            Assert.AreEqual(20.0, o.Item.Height);
        }

        [TestMethod]
        public void GenericResolution()
        {
            var o = Factory.Create("toast-me", 3, "Hello") as Activator_Tests.ClassWithGeneric<int, string>;

            Assert.AreEqual(3, o.Property1);
            Assert.AreEqual("Hello", o.Property2);
        }
    }
}