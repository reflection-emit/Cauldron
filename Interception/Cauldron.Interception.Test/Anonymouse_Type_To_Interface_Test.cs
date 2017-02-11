using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Anonymouse_Type_To_Interface_Test
    {
        public interface IKeyValue
        {
            ITestInterface[] Items { get; }

            string Key { get; }
        }

        [TestMethod]
        public void Anonymouse_Type_Complex()
        {
            var list = new List<TestClass>()
            {
                new TestClass { StringProperty = "Test 1", DoubleProperty = 898.9, IntegerProperty = 9 },
                new TestClass { StringProperty = "Test 2", DoubleProperty = 2134, IntegerProperty = 4 },
                new TestClass { StringProperty = "Test 5", DoubleProperty = 33, IntegerProperty = 4 },
                new TestClass { StringProperty = "Test 1", DoubleProperty = 213, IntegerProperty = 543 },
                new TestClass { StringProperty = "Test 5", DoubleProperty = 234, IntegerProperty = 45 },
                new TestClass { StringProperty = "Test 1", DoubleProperty = 38.9, IntegerProperty = 45 },
                new TestClass { StringProperty = "Test 2", DoubleProperty = 546.76, IntegerProperty = 34 },
                new TestClass { StringProperty = "Test 12", DoubleProperty = 66.67, IntegerProperty = 45 },
                new TestClass { StringProperty = "Test 2", DoubleProperty = 9.88, IntegerProperty = 345 },
                new TestClass { StringProperty = "Test 2", DoubleProperty = .9, IntegerProperty = 345 },
            };

            var obj = list
                .GroupBy(x => x.StringProperty)
                .Select(x => new { Key = x.Key, Items = x.ToArray<ITestInterface>() })
                .OrderBy(x => x.Key)
                .Where(x => x.Items.Length > 3)
                .Select(x => x.CreateObject<IKeyValue>())
                .ToList();

            Assert.AreEqual(1, obj.Count());
            Assert.AreEqual(4, obj[0].Items.Length);
            Assert.AreEqual("Test 2", obj[0].Key);
        }

        [TestMethod]
        public void Anonymouse_Type_Verify()
        {
            var obj = new { DoubleProperty = 5.5, LongProperty = 60L, StringProperty = "Hello" }.CreateObject<ITestInterface>();

            Assert.AreEqual("Hello", obj.StringProperty);
            Assert.AreEqual(5.5, obj.DoubleProperty);
            Assert.AreEqual(60, obj.LongProperty);
        }
    }
}