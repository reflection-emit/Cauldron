using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cauldron.Desktop.Activator.Test
{
    [TestClass]
    public class Inject_Test
    {
        [Inject]
        private ITestInterface[] injectToArray = null;

        [Inject]
        private Collection<ITestInterface> injectToCollection = null;

        [Inject]
        private IEnumerable<ITestInterface> injectToEnumerable = null;

        //[Inject]
        //private KeyedCollection<string, ITestInterface> injectToKeyedCollection = null;

        [Inject]
        private List<ITestInterface> injectToList = null;

        //[Inject]
        //public Dictionary<string, ITestInterface> InjectToDictionary { get; private set; }

        [Inject]
        public ITestInterface InterfaceInject { get; private set; }

        [TestMethod]
        public void Array_Injection()
        {
            var documentMock1 = new Mock<ITestInterface>();
            var documentMock2 = new Mock<ITestInterface>();

            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock1.Object.GetType());
            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock2.Object.GetType());

            Assert.AreEqual(3, this.injectToArray.Length);

            this.injectToArray[0].Height = 66;

            Assert.AreEqual(66, this.injectToArray[0].Height);

            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock1.Object.GetType());
            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock2.Object.GetType());
        }

        [TestMethod]
        public void Collection_Injection()
        {
            var documentMock1 = new Mock<ITestInterface>();
            var documentMock2 = new Mock<ITestInterface>();

            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock1.Object.GetType());
            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock2.Object.GetType());

            Assert.AreEqual(3, this.injectToCollection.Count);

            this.injectToCollection[0].Height = 66;

            Assert.AreEqual(66, this.injectToCollection[0].Height);

            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock1.Object.GetType());
            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock2.Object.GetType());
        }

        [TestMethod]
        public void IEnumerable_Injection()
        {
            var documentMock1 = new Mock<ITestInterface>();
            var documentMock2 = new Mock<ITestInterface>();

            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock1.Object.GetType());
            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock2.Object.GetType());

            Assert.AreEqual(3, this.injectToEnumerable.Count());

            this.injectToEnumerable.ElementAt(0).Height = 66;

            Assert.AreEqual(66, this.injectToEnumerable.ElementAt(0).Height);

            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock1.Object.GetType());
            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock2.Object.GetType());
        }

        [TestMethod]
        public void List_Injection()
        {
            var documentMock1 = new Mock<ITestInterface>();
            var documentMock2 = new Mock<ITestInterface>();

            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock1.Object.GetType());
            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock2.Object.GetType());

            Assert.AreEqual(3, this.injectToList.Count);

            this.injectToList[0].Height = 66;

            Assert.AreEqual(66, this.injectToList[0].Height);

            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock1.Object.GetType());
            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock2.Object.GetType());
        }

        [TestMethod]
        public void Object_Injection()
        {
            this.InterfaceInject.Height = 66;
            Assert.AreEqual(66, InterfaceInject.Height);

            foreach (var item in this.injectToList)
            {
                injectToList.Add(item as ITestInterface);
            }
        }
    }
}