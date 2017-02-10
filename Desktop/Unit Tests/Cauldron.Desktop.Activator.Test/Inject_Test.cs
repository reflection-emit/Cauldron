using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cauldron.Desktop.Activator.Test
{
    [TestClass]
    public class Inject_Test
    {
        private KeyedTestList<string, int, ITestInterface> bjbj;

        [Inject]
        private ITestInterface[] injectToArray = null;

        [Inject]
        private Collection<ITestInterface> injectToCollection = null;

        [Inject]
        private IEnumerable<ITestInterface> injectToEnumerable = null;

        [Inject]
        private KeyedTestList<string, int, ITestInterface> injectToKeyedCollection = null;

        [Inject]
        private List<ITestInterface> injectToList = null;

        private Collection<string> test;

        [Inject]
        public Dictionary<string, float> InjectToDictionary { get; private set; }

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
        public void Dictionary_Injection()
        {
            var dictionaryMock = new Dictionary<string, float>();

            Factory.AddType(typeof(Dictionary<string, float>).FullName, FactoryCreationPolicy.Singleton, dictionaryMock.GetType());
            dictionaryMock = Factory.Create(typeof(Dictionary<string, float>).FullName) as Dictionary<string, float>;

            dictionaryMock.Add("Hi", 33.6f);
            dictionaryMock.Add("Cool", 324f);
            dictionaryMock.Add("A new Type", 456.34f);
            dictionaryMock.Add("Generic", 34.88f);

            Assert.AreEqual(4, this.InjectToDictionary.Count);
            Assert.AreEqual(324f, this.InjectToDictionary["Cool"]);
            Assert.AreEqual(34.88f, this.InjectToDictionary["Generic"]);
            Assert.AreEqual(33.6f, this.InjectToDictionary["Hi"]);

            this.InjectToDictionary["Cool"] = 55f;
            this.InjectToDictionary["Generic"] = 66f;
            this.InjectToDictionary["Hi"] = 99.9f;

            Assert.AreEqual(55f, this.InjectToDictionary["Cool"]);
            Assert.AreEqual(66f, this.InjectToDictionary["Generic"]);
            Assert.AreEqual(99.9f, this.InjectToDictionary["Hi"]);

            Factory.Destroy(typeof(Dictionary<string, float>).FullName);
            Factory.RemoveType(typeof(Dictionary<string, float>).FullName, dictionaryMock.GetType());
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
        public void KeyedCollection_Injection()
        {
            var documentMock1 = new Mock<ITestInterface>();

            Factory.AddType(typeof(ITestInterface).FullName, FactoryCreationPolicy.Instanced, documentMock1.Object.GetType());

            Assert.AreEqual(2, this.injectToKeyedCollection.Count);

            this.injectToKeyedCollection[0].Height = 66;

            Assert.AreEqual(66, this.injectToKeyedCollection[0].Height);

            Factory.RemoveType(typeof(ITestInterface).FullName, documentMock1.Object.GetType());
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
        }

        private void GZGZG()
        {
            if (test.Count > 9)
            {
                test.Clear();
                return;
            }
            test.Add("zuzu");
            if (bjbj.Count == 2)
            {
                bjbj.Clear();
            }
        }
    }
}