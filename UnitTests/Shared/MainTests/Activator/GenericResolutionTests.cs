using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

[module: GenericComponent(typeof(UnitTests.Activator.ClassWithGeneric<int, string>), typeof(UnitTests.Activator.ClassWithGeneric<int, string>))]
[module: GenericComponent(typeof(UnitTests.Activator.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(UnitTests.Activator.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(UnitTests.Activator.ClassWithGeneric<int, string>), "toast-me")]
[module: GenericComponent(typeof(UnitTests.Activator.GenericClassWithStaticCtor<UnitTests.Activator.TestClass>), typeof(UnitTests.Activator.IGenericClassWithStaticCtor<UnitTests.Activator.TestClass>))]
namespace UnitTests.Activator
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

        [ComponentConstructor]
        public ClassWithGeneric(IEnumerable<T2> ts)
        {
            this.Items = ts.ToArray();
        }

        [ComponentConstructor]
        public ClassWithGeneric(T1 a, T2 b)
        {
            this.Property1 = a;
            this.Property2 = b;
        }

        public T2[] Items { get; }
        public T1 Property1 { get; set; }

        public T2 Property2 { get; set; }
    }

    public class GenericClassWithStaticCtor<T> : IGenericClassWithStaticCtor<T> where T : new()
    {
        private GenericClassWithStaticCtor()
        {
            this.Item = new T();
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
            var o = Factory.Create("toast-me", 3, "Hello") as UnitTests.Activator.ClassWithGeneric<int, string>;

            Assert.AreEqual(3, o.Property1);
            Assert.AreEqual("Hello", o.Property2);
        }

        [TestMethod]
        public void Insure_That_Generic_Casting_Is_Correctly_Weaved()
        {
            var stuff = new string[]
            {
                "Hello",
                "Whats",
                "Up"
            };

            var result = Factory.Create(typeof(ClassWithGeneric<int, string>), new object[] { stuff }) as ClassWithGeneric<int, string>;
            Assert.AreEqual("Hello", result.Items[0]);
            Assert.AreEqual("Whats", result.Items[1]);
            Assert.AreEqual("Up", result.Items[2]);
        }
    }
}