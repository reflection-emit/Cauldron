using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.Activator;
using System.Collections;
using Cauldron;
using Cauldron.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Activator_Tests
{
    [TestClass]
    public class MappingTest
    {
        [TestMethod]
        public void Map_Class_With_Fields_Test()
        {
            var source = new TestMappingInterface1();
            source.Property1 = 99;
            source.field = "Hello";

            var target = source.MapTo(new TestMappingInterface1());

            Assert.IsFalse(Object.ReferenceEquals(source, target));

            Assert.AreEqual(target.Property1, source.Property1);
            Assert.AreEqual(target.field, source.field);
        }

        [TestMethod]
        public void Map_Class_With_Struct_Test()
        {
            var source = new TestMappingInterface1();
            source.EinStruct = new TestMappingStruct
            {
                field = 88,
                Value = 2
            };

            var target = source.MapTo(new TestMappingInterface1());

            Assert.IsFalse(Object.ReferenceEquals(source, target));

            Assert.AreEqual(target.EinStruct.Value, source.EinStruct.Value);
            Assert.AreEqual(target.EinStruct.field, source.EinStruct.field);
        }

        [TestMethod]
        public void Map_Collections_Test()
        {
            var source = new TestMappingWithCollectionAndIEnumerations();
            source.List = new List<ITestMappingInterface>
            {
                new TestMappingInterface1
                {
                    EinStruct = new TestMappingStruct { field = 9, Value = 3 },
                    field = "Test",
                    Property1 = 44
                },
                new TestMappingInterface2 { Property1 = 77 }
            };
            source.IEnumerableT = new List<ITestMappingInterface>
            {
                new TestMappingInterface1
                {
                    EinStruct = new TestMappingStruct { field = 7, Value = 2 },
                    field = "Muhahaha",
                    Property1 = 22
                },
                new TestMappingInterface2 { Property1 = 22 }
            };
            source.IEnumerable = new List<ITestMappingInterface>
            {
                new TestMappingInterface1
                {
                    EinStruct = new TestMappingStruct { field = 1, Value = 99 },
                    field = "Get Lucky",
                    Property1 = 33
                },
                new TestMappingInterface2 { Property1 = 88 }
            };
            source.Dictionary = new Dictionary<string, ITestMappingInterface>();
            source.Dictionary.Add("number1", new TestMappingInterface1
            {
                EinStruct = new TestMappingStruct { field = 9, Value = 3 },
                field = "Test",
                Property1 = 44
            });
            source.Dictionary.Add("number2", new TestMappingInterface2 { Property1 = 77 });
            source.Array = new ITestMappingInterface[]
            {
                new TestMappingInterface1
                {
                    EinStruct = new TestMappingStruct { field = 1, Value = 99 },
                    field = "Get Lucky",
                    Property1 = 33
                },
                new TestMappingInterface2 { Property1 = 88 }
            };

            var target = source.MapTo<TestMappingWithCollectionAndIEnumerations>();

            Assert.IsFalse(Object.ReferenceEquals(source, target));
            Assert.IsFalse(Object.ReferenceEquals(source.List, target.List));
            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerableT, target.IEnumerableT));
            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerable, target.IEnumerable));
            Assert.IsFalse(Object.ReferenceEquals(source.Dictionary, target.Dictionary));
            Assert.IsFalse(Object.ReferenceEquals(source.Array, target.Array));

            Assert.AreEqual(source.List.Count, target.List.Count);
            Assert.AreEqual(source.IEnumerableT.Count(), target.IEnumerableT.Count());
            Assert.AreEqual(source.IEnumerable.Operations().Count(), target.IEnumerable.Operations().Count());
            Assert.AreEqual(source.Dictionary.Count, target.Dictionary.Count);
            Assert.AreEqual(source.Array.Length, target.Array.Length);

            Assert.IsFalse(Object.ReferenceEquals(source.List[0], target.List[0]));
            Assert.IsFalse(Object.ReferenceEquals(source.List[1], target.List[1]));
            Assert.AreEqual(source.List[0].Property1, target.List[0].Property1);
            Assert.AreEqual(source.List[0].As<TestMappingInterface1>().field, target.List[0].As<TestMappingInterface1>().field);
            Assert.AreEqual(source.List[0].As<TestMappingInterface1>().EinStruct.field, target.List[0].As<TestMappingInterface1>().EinStruct.field);
            Assert.AreEqual(source.List[0].As<TestMappingInterface1>().EinStruct.Value, target.List[0].As<TestMappingInterface1>().EinStruct.Value);
            Assert.AreEqual(source.List[1].Property1, target.List[1].Property1);

            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerableT.ElementAt(0), target.IEnumerableT.ElementAt(0)));
            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerableT.ElementAt(1), target.IEnumerableT.ElementAt(1)));
            Assert.AreEqual(source.IEnumerableT.ElementAt(0).Property1, target.IEnumerableT.ElementAt(0).Property1);
            Assert.AreEqual(source.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().field, target.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().field);
            Assert.AreEqual(source.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().EinStruct.field, target.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().EinStruct.field);
            Assert.AreEqual(source.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().EinStruct.Value, target.IEnumerableT.ElementAt(0).As<TestMappingInterface1>().EinStruct.Value);
            Assert.AreEqual(source.IEnumerableT.ElementAt(1).Property1, target.IEnumerableT.ElementAt(1).Property1);

            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerable.Operations().ElementAt(0), target.IEnumerable.Operations().ElementAt(0)));
            Assert.IsFalse(Object.ReferenceEquals(source.IEnumerable.Operations().ElementAt(1), target.IEnumerable.Operations().ElementAt(1)));
            Assert.AreEqual(source.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().Property1, target.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().Property1);
            Assert.AreEqual(source.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().field, target.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().field);
            Assert.AreEqual(source.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().EinStruct.field, target.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().EinStruct.field);
            Assert.AreEqual(source.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().EinStruct.Value, target.IEnumerable.Operations().ElementAt(0).As<TestMappingInterface1>().EinStruct.Value);
            Assert.AreEqual(source.IEnumerable.Operations().ElementAt(1).As<TestMappingInterface2>().Property1, target.IEnumerable.Operations().ElementAt(1).As<TestMappingInterface2>().Property1);

            Assert.IsFalse(Object.ReferenceEquals(source.Array[0], target.Array[0]));
            Assert.IsFalse(Object.ReferenceEquals(source.Array[1], target.Array[1]));
            Assert.AreEqual(source.Array[0].Property1, target.Array[0].Property1);
            Assert.AreEqual(source.Array[0].As<TestMappingInterface1>().field, target.Array[0].As<TestMappingInterface1>().field);
            Assert.AreEqual(source.Array[0].As<TestMappingInterface1>().EinStruct.field, target.Array[0].As<TestMappingInterface1>().EinStruct.field);
            Assert.AreEqual(source.Array[0].As<TestMappingInterface1>().EinStruct.Value, target.Array[0].As<TestMappingInterface1>().EinStruct.Value);
            Assert.AreEqual(source.Array[1].Property1, target.Array[1].Property1);

            Assert.IsFalse(Object.ReferenceEquals(source.Dictionary.ElementAt(0).Key, target.Dictionary.ElementAt(0).Key));
            Assert.IsFalse(Object.ReferenceEquals(source.Dictionary.ElementAt(1).Key, target.Dictionary.ElementAt(1).Key));
            Assert.IsFalse(Object.ReferenceEquals(source.Dictionary.ElementAt(0).Value, target.Dictionary.ElementAt(0).Value));
            Assert.IsFalse(Object.ReferenceEquals(source.Dictionary.ElementAt(1).Value, target.Dictionary.ElementAt(1).Value));
            Assert.AreEqual(source.Dictionary.ElementAt(0).Value.Property1, target.Dictionary.ElementAt(0).Value.Property1);
            Assert.AreEqual(source.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().field, target.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().field);
            Assert.AreEqual(source.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().EinStruct.field, target.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().EinStruct.field);
            Assert.AreEqual(source.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().EinStruct.Value, target.Dictionary.ElementAt(0).Value.As<TestMappingInterface1>().EinStruct.Value);
            Assert.AreEqual(source.Dictionary.ElementAt(1).Value.Property1, target.Dictionary.ElementAt(1).Value.Property1);
        }

        [TestMethod]
        public void Map_Complex_Class_Test()
        {
            var source = new TestMappingWithStructsAndClasses();
            source.SomeProperty = new TestMappingInterface1
            {
                EinStruct = new TestMappingStruct { field = 9, Value = 3 },
                field = "Test",
                Property1 = 44
            };
            source.guid = Guid.NewGuid();
            source.Date = DateTime.Now;

            var target = source.MapTo<TestMappingWithStructsAndClasses>();

            Assert.IsFalse(Object.ReferenceEquals(source, target));
            Assert.IsFalse(Object.ReferenceEquals(source.SomeProperty, target.SomeProperty));

            Assert.AreEqual(target.guid, source.guid);
            Assert.AreEqual(target.SomeProperty.field, source.SomeProperty.field);
            Assert.AreEqual(target.SomeProperty.Property1, source.SomeProperty.Property1);
            Assert.AreEqual(target.SomeProperty.EinStruct.field, source.SomeProperty.EinStruct.field);
            Assert.AreEqual(target.SomeProperty.EinStruct.Value, source.SomeProperty.EinStruct.Value);
        }

        [TestMethod]
        public void Map_Primitiv_Strings_Arrays_Test()
        {
            var source = new ClassWithPrimitivesAndStringsArrays();
            source.CharArray = new char[] { 'o', 't', '7' };
            source.DoubleArray = new double[] { 3.0, 9.22, 3.14 };
            source.IntArray = new int[] { 8, 9, 3 };
            source.StringArray = new string[] { "Hello", "People", "Lore" };

            var target = source.DeepClone();

            Assert.IsFalse(Object.ReferenceEquals(source, target));
            Assert.IsFalse(Object.ReferenceEquals(source.CharArray, target.CharArray));
            Assert.IsFalse(Object.ReferenceEquals(source.DoubleArray, target.DoubleArray));
            Assert.IsFalse(Object.ReferenceEquals(source.IntArray, target.IntArray));
            Assert.IsFalse(Object.ReferenceEquals(source.StringArray, target.StringArray));

            Assert.AreEqual(target.CharArray.Length, source.CharArray.Length);
            Assert.AreEqual(target.DoubleArray.Length, source.DoubleArray.Length);
            Assert.AreEqual(target.IntArray.Length, source.IntArray.Length);
            Assert.AreEqual(target.StringArray.Length, source.StringArray.Length);

            Assert.AreEqual(target.CharArray[0], source.CharArray[0]);
            Assert.AreEqual(target.CharArray[1], source.CharArray[1]);
            Assert.AreEqual(target.CharArray[2], source.CharArray[2]);

            Assert.AreEqual(target.DoubleArray[0], source.DoubleArray[0]);
            Assert.AreEqual(target.DoubleArray[1], source.DoubleArray[1]);
            Assert.AreEqual(target.DoubleArray[2], source.DoubleArray[2]);

            Assert.AreEqual(target.IntArray[0], source.IntArray[0]);
            Assert.AreEqual(target.IntArray[1], source.IntArray[1]);
            Assert.AreEqual(target.IntArray[2], source.IntArray[2]);

            Assert.AreEqual(target.StringArray[0], source.StringArray[0]);
            Assert.AreEqual(target.StringArray[1], source.StringArray[1]);
            Assert.AreEqual(target.StringArray[2], source.StringArray[2]);

            Assert.IsFalse(Object.ReferenceEquals(source.StringArray[0], target.StringArray[0]));
            Assert.IsFalse(Object.ReferenceEquals(source.StringArray[1], target.StringArray[1]));
            Assert.IsFalse(Object.ReferenceEquals(source.StringArray[2], target.StringArray[2]));
        }

        [TestMethod]
        public void Map_Simple_Class_Test()
        {
            var source = new TestMappingInterface2();
            source.Property1 = 99;

            var target = source.MapTo(new TestMappingInterface2());

            Assert.IsFalse(Object.ReferenceEquals(source, target));

            Assert.AreEqual(target.Property1, source.Property1);
        }

        [TestMethod]
        public void Map_Simple_Struct_Test()
        {
            var source = new TestMappingStruct();
            source.Value = 99;
            source.field = 1.2;

            var target = source.MapTo(new TestMappingStruct());

            Assert.IsFalse(Object.ReferenceEquals(source, target));

            Assert.AreEqual(target.Value, source.Value);
            Assert.AreEqual(target.field, source.field);
        }
    }

    #region Resources

    public interface ITestMappingInterface
    {
        int Property1 { get; set; }
    }

    public struct TestMappingStruct
    {
        public double field;

        public int Value { get; set; }
    }

    public class ClassWithoutParameterlessConstructor
    {
        public ClassWithoutParameterlessConstructor(int value)
        {
            this.DefaultValue = value;
        }

        public int DefaultValue { get; set; } = 70;
    }

    public class ClassWithPrimitivesAndStringsArrays
    {
        public char[] CharArray { get; set; }

        public double[] DoubleArray { get; set; }

        public int[] IntArray { get; set; }

        public string[] StringArray { get; set; }
    }

    public class TestMappingInterface1 : ITestMappingInterface
    {
        public string field;

        public TestMappingStruct EinStruct { get; set; }

        public int Property1 { get; set; }
    }

    public class TestMappingInterface2 : ITestMappingInterface
    {
        public int Property1 { get; set; }
    }

    public class TestMappingWithCollectionAndIEnumerations
    {
        public ITestMappingInterface[] Array { get; set; }

        public Dictionary<string, ITestMappingInterface> Dictionary { get; set; }

        public IEnumerable IEnumerable { get; set; }

        public IEnumerable<ITestMappingInterface> IEnumerableT { get; set; }

        public List<ITestMappingInterface> List { get; set; }
    }

    public class TestMappingWithStructsAndClasses
    {
        public Guid guid;

        public DateTime Date { get; set; }

        public TestMappingInterface1 SomeProperty { get; set; }
    }

    #endregion Resources
}