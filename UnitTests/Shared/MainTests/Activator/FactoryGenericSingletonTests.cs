using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Activator
{
    [TestClass]
    public class FactoryGenericSingletonTests
    {
        public interface IMyClass
        {
            string Value { get; set; }
        }

        [TestMethod]
        public void Create_Instance_Class()
        {
            Factory.Create<MyClass>().Value = "Toast";

            var instance = Factory.Create<MyClass>();

            Assert.AreEqual(Factory.Create<MyClass>().Value, instance.Value);
        }

        [TestMethod]
        public void Create_Instance_Interface()
        {
            Factory.Create<IMyClass>().Value = "Toast";

            var instance = Factory.Create<IMyClass>();

            Assert.AreEqual(Factory.Create<IMyClass>().Value, instance.Value);
        }

        [Component(typeof(MyClass), FactoryCreationPolicy.Singleton)]
        public class MyClass
        {
            public string Value { get; set; }
        }

        [Component(typeof(IMyClass), FactoryCreationPolicy.Singleton)]
        public class MySecondClass : IMyClass
        {
            public string Value { get; set; }
        }
    }
}