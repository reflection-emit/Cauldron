using Cauldron.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.Test
{
    [TestClass]
    public class ConcurrentListTest
    {
        [TestMethod]
        public void Remove_Multiple_Test()
        {
            var list = new ConcurrentList<string>();

            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 3");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");

            list.Remove(x => x == "Item 1");

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void Remove_Multiple_Test_2()
        {
            var list = new ConcurrentList<string>();

            list.Add("Item 4");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 3");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 0");

            list.Remove(x => x == "Item 1");

            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void Remove_Multiple_Test_3()
        {
            var list = new ConcurrentList<string>();

            list.Add("Item 4");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 3");
            list.Add("Item 1");
            list.Add("Item 1");
            list.Add("Item 0");

            list.Remove(x => x.StartsWith("Item"));

            Assert.AreEqual(0, list.Count);
        }
    }
}