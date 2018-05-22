using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[module: GenericComponent(typeof(Activator_Tests.ClassWithGeneric<int, string>), "toast-me")]

namespace Activator_Tests
{
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

    [TestClass]
    public class GenericResolutionTests
    {
        [TestMethod]
        public void GenericResolution()
        {
            var o = Factory.Create("toast-me", 3, "Hello") as Activator_Tests.ClassWithGeneric<int, string>;

            Assert.AreEqual(3, o.Property1);
            Assert.AreEqual("Hello", o.Property2);
        }
    }
}