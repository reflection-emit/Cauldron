using Activator_Tests;
using Cauldron;
using Cauldron.Activator;
using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Activator_Tests
{
    [TestClass]
    public class Inject_Test
    {
        [Inject(NoPreloading = true)]
        private ITestInterface[] injectToArray = null;

        [Inject(NoPreloading = true)]
        private Collection<ITestInterface> injectToCollection = null;

        [Inject(NoPreloading = true)]
        private IEnumerable<ITestInterface> injectToEnumerable = null;

        [Inject(NoPreloading = true)]
        private KeyedTestList<string, int, ITestInterface> injectToKeyedCollection = null;

        [Inject(NoPreloading = true)]
        private List<ITestInterface> injectToList = null;

        [Inject(ContractType = typeof(ITestInterface), NoPreloading = true)]
        public object CustomName1 { get; }

        [Inject(ContractName = "Muhahaha", NoPreloading = true)]
        public object CustomName2 { get; }

        [Inject(NoPreloading = true)]
        public Dictionary<string, float> InjectToDictionary { get; private set; }

        [Inject(NoPreloading = true)]
        public ITestInterface InterfaceInject { get; private set; }

        [Inject("[property] InterfaceInject", "[this]", 90.9, "Hello", true, "[field] injectToList", 0x89, ContractName = "Blub")]
        public ITestInterface InterfaceInjectWithParameters { get; private set; }

        [Inject(MakeThreadSafe = true, NoPreloading = true)]
        public ITestInterface ThreadSafeInject { get; private set; }

        [TestMethod]
        public void Array_Injection()
        {
            var documentMock1 = new { }.CreateType<ITestInterface>();
            var documentMock2 = new { }.CreateType<ITestInterface>();
            var documentMock1Type = documentMock1.GetType();
            var documentMock2Type = documentMock2.GetType();

            try
            {
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock1Type, x => documentMock1Type.CreateInstance());
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock2Type, x => documentMock2Type.CreateInstance());

                Assert.AreEqual(3, this.injectToArray.Length);

                this.injectToArray[0].Height = 66;

                Assert.AreEqual(66, this.injectToArray[0].Height);
            }
            finally
            {
                Factory.RemoveType(typeof(ITestInterface), documentMock1Type);
                Factory.RemoveType(typeof(ITestInterface), documentMock2Type);
            }
        }

        [TestMethod]
        public void Collection_Injection()
        {
            var documentMock1 = new { }.CreateType<ITestInterface>();
            var documentMock2 = new { }.CreateType<ITestInterface>();
            var documentMock1Type = documentMock1.GetType();
            var documentMock2Type = documentMock2.GetType();

            try
            {
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock1Type, x => documentMock1Type.CreateInstance());
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock2Type, x => documentMock2Type.CreateInstance());

                Assert.AreEqual(3, this.injectToCollection.Count);

                this.injectToCollection[0].Height = 66;

                Assert.AreEqual(66, this.injectToCollection[0].Height);
            }
            finally
            {
                Factory.RemoveType(typeof(ITestInterface), documentMock1Type);
                Factory.RemoveType(typeof(ITestInterface), documentMock2Type);
            }
        }

        [TestMethod]
        public void Singleton_Injection()
        {
            var stuff = Factory.Create("Hello");
            Assert.AreNotEqual(null, stuff);
        }

        [TestMethod]
        public void Dictionary_Injection()
        {
            var dictionaryMock = new Dictionary<string, float>();
            var documentMockType = dictionaryMock.GetType();

            try
            {
                Factory.AddType(typeof(Dictionary<string, float>), FactoryCreationPolicy.Singleton, dictionaryMock.GetType(), x => documentMockType.CreateInstance());
                dictionaryMock = Factory.Create(typeof(Dictionary<string, float>)) as Dictionary<string, float>;

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
            }
            finally
            {
                Factory.Destroy(typeof(Dictionary<string, float>));
                Factory.RemoveType(typeof(Dictionary<string, float>), documentMockType);
            }
        }

        [TestMethod]
        public void IEnumerable_Injection()
        {
            var documentMock1 = new { }.CreateType<ITestInterface>();
            var documentMock2 = new { }.CreateType<ITestInterface>();
            var documentMock1Type = documentMock1.GetType();
            var documentMock2Type = documentMock2.GetType();

            try
            {
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock1Type, x => documentMock1Type.CreateInstance());
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock2Type, x => documentMock2Type.CreateInstance());

                Assert.AreEqual(3, this.injectToEnumerable.Count());
                var tt = this.injectToEnumerable.ElementAt(0);
                tt.Height = 88;

                this.injectToEnumerable.ElementAt(0).Height = 66;

                Assert.AreEqual(66, this.injectToEnumerable.ElementAt(0).Height);
            }
            finally
            {
                Factory.RemoveType(typeof(ITestInterface), documentMock1Type);
                Factory.RemoveType(typeof(ITestInterface), documentMock2Type);
            }
        }

        [TestMethod]
        public void KeyedCollection_Injection()
        {
            var dictionaryMock = new { }.CreateType<ITestInterface>();
            var documentMockType = dictionaryMock.GetType();

            try
            {
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Singleton, dictionaryMock.GetType(), x => documentMockType.CreateInstance());

                Assert.AreEqual(2, this.injectToKeyedCollection.Count);

                this.injectToKeyedCollection[0].Height = 66;

                Assert.AreEqual(66, this.injectToKeyedCollection[0].Height);
            }
            finally
            {
                Factory.RemoveType(typeof(ITestInterface), documentMockType);
            }
        }

        [TestMethod]
        public void List_Injection()
        {
            var documentMock1 = new { }.CreateType<ITestInterface>();
            var documentMock2 = new { }.CreateType<ITestInterface>();
            var documentMock1Type = documentMock1.GetType();
            var documentMock2Type = documentMock2.GetType();

            try
            {
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock1Type, x => documentMock1Type.CreateInstance());
                Factory.AddType(typeof(ITestInterface), FactoryCreationPolicy.Instanced, documentMock2Type, x => documentMock2Type.CreateInstance());

                Assert.AreEqual(3, this.injectToList.Count);

                this.injectToList[0].Height = 66;

                Assert.AreEqual(66, this.injectToList[0].Height);
            }
            finally
            {
                Factory.RemoveType(typeof(ITestInterface), documentMock1Type);
                Factory.RemoveType(typeof(ITestInterface), documentMock2Type);
            }
        }

        [TestMethod]
        public void NestedType_Reference_To_Parent_Injection()
        {
            var parent = new NestedClassParent();
            var child = new NestedClassParent.NestedClassChild(parent);

            // This will throw an null exception

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Object_Injection()
        {
            this.InterfaceInject.Height = 66;
            Assert.AreEqual(66, InterfaceInject.Height);
        }
    }
}