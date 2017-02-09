using Cauldron.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class ChildOf_Implementer_Test
    {
        [TestMethod]
        public void ChildOf_KeyedCollection()
        {
            Assert.AreEqual(typeof(int), Reflection.ChildTypeOf(typeof(KeyedCollectionTest)));
        }

        [TestMethod]
        public void ChildOf_List_string()
        {
            Assert.AreEqual(typeof(string), Reflection.ChildTypeOf(typeof(List<string>)));
        }
    }

    public class KeyedCollectionTest : KeyedCollection<string, int>
    {
        protected override string GetKeyForItem(int item) => item.ToString();
    }
}