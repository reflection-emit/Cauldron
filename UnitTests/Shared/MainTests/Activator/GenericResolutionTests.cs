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

    public interface IClassWithGeneric<T1, T2>
    {
        T1 Property1 { get; }
        T2 Property2 { get; }
    }

    [Component(typeof(IClassWithGeneric<,>))]
    public class ClassWithGeneric<T1, T2> : IClassWithGeneric<T1, T2>
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
        public void Generic_Auto_Resolution()
        {
            var o = Factory.Create<IClassWithGeneric<string,long>>("Hello", 777L);

            Assert.AreEqual("Hello", o.Property1);
            Assert.AreEqual(777L, o.Property2);
        }

        [TestMethod]
        public void Generic_Auto_Multiple_Resolution()
        {
            var o1 = Factory.Create<IClassWithGeneric<string,long>>("Hello", 777L);
            var o2 = Factory.Create<IClassWithGeneric<string,int>>("Hello", 33);
            var o3 = Factory.Create<IClassWithGeneric<int,int>>(88, 88);
            var o4 = Factory.Create<IClassWithGeneric<long,int>>(23L, 565);
            var o5 = Factory.Create<IClassWithGeneric<double,double>>(34.5, 22.3);
            var o6 = Factory.Create<IClassWithGeneric<long,long>>(99L, 22L);
            var o7 = Factory.Create<IClassWithGeneric<string,string>>("Hello", "Lop");

            Assert.AreEqual("Hello", o1.Property1);
            Assert.AreEqual(777L, o1.Property2);

            Assert.AreEqual("Hello", o2.Property1);
            Assert.AreEqual(33, o2.Property2);

            Assert.AreEqual(88, o3.Property1);
            Assert.AreEqual(88, o3.Property2);

            Assert.AreEqual(23L, o4.Property1);
            Assert.AreEqual(565, o4.Property2);

            Assert.AreEqual(34.5, o5.Property1);
            Assert.AreEqual(22.3, o5.Property2);

            Assert.AreEqual(99L, o6.Property1);
            Assert.AreEqual(22L, o6.Property2);

            Assert.AreEqual("Hello", o7.Property1);
            Assert.AreEqual("Lop", o7.Property2);
        }

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