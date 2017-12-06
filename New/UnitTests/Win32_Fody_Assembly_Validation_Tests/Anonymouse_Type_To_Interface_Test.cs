using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win32_Fody_Assembly_Validation_Tests
{
    public interface IAnonymousClassWithInterfaceExtension2TestInterface : IAnonymousClassWithInterfaceExtensionTestInterface
    {
        DateTime AnyDate { get; }

        string FirstProperty { get; }

        IAnonymousClassWithInterfaceExtensionTestInterface Interface { get; set; }
    }

    public interface IAnonymousClassWithInterfaceExtensionTestInterface
    {
        double ADouble { get; }

        int AnyInt { get; }

        string TheString { get; }
    }

    public interface IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod
    {
        double ADouble { get; }

        int AnyInt { get; }

        string TheString { get; }

        string GetAnyStringMethod(int aParameter, char bParameter);
    }

    public interface IHasGenericInterfaces : IEquatable<string>, IEquatable<int>
    {
        int AnyInt { get; }
    }

    public interface IInterfaceWithNullableTypesAndGenericTypes
    {
        GenericType<double> AnyDouble { get; }
        int? AnyInt { get; }
    }

    public interface ISpecial
    {
        string Description { get; }
        string Name { get; }
    }

    public static class Anonymouse_Type_To_Interface_Test_Extensions
    {
        public static TResult Blabla<T, TResult>(this T value, Func<T, TResult> function) => function(value);
    }

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
                .Select(x => x.CreateType<IKeyValue>())
                .ToList();

            Assert.AreEqual(1, obj.Count());
            Assert.AreEqual(4, obj[0].Items.Length);
            Assert.AreEqual("Test 2", obj[0].Key);
        }

        [TestMethod]
        public void Anonymouse_Type_Generic_Types()
        {
            var item = new
            {
                AnyDouble = new GenericType<double> { Value = 9.4 },
                AnyInt = (int?)20
            }.CreateType<IInterfaceWithNullableTypesAndGenericTypes>();

            Assert.AreEqual(9.4, item.AnyDouble.Value);
            Assert.AreEqual(20, item.AnyInt);
        }

        [TestMethod]
        public void Anonymouse_Type_Verify()
        {
            var obj = new { DoubleProperty = 5.5, LongProperty = 60L, StringProperty = "Hello" }.CreateType<ITestInterface>();

            Assert.AreEqual("Hello", obj.StringProperty);
            Assert.AreEqual(5.5, obj.DoubleProperty);
            Assert.AreEqual(60, obj.LongProperty);
        }

        [TestMethod]
        public void Anonymouse_With_GenericInterface()
        {
            var obj = new { AnyInt = 9 }.CreateType<IHasGenericInterfaces>();
            Assert.AreEqual(9, obj.AnyInt);
        }

        [TestMethod]
        public void Bug38_Validator()
        {
            var blub = "Hi;This;Is;Me";
            var huhu = blub.Split(';')
                .Select(x => new { Name = x[3], Description = x[1] })
                .CreateType<ISpecial>();
        }

        [TestMethod]
        public void Complex_Type_Creation_Test()
        {
            var anonClass = new { AnyDate = DateTime.Now, FirstProperty = "TestString" };
            var newObject = anonClass.CreateType<IAnonymousClassWithInterfaceExtension2TestInterface>();

            Assert.IsTrue(anonClass.AnyDate == newObject.AnyDate);
            Assert.IsTrue(anonClass.FirstProperty == newObject.FirstProperty);
            Assert.IsTrue(newObject.Interface == null);
        }

        [TestMethod]
        public void Nested_Class_Test()
        {
            var nested = new OuterClass.NestedClass();
            var value = nested.GetValues();

            Assert.AreEqual(6666.666, value.ADouble);
            Assert.AreEqual(666, value.AnyInt);
            Assert.AreEqual("TestString", value.TheString);
        }

        [TestMethod]
        public void Simple_Type_Creation_Test()
        {
            var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
            var newObject = anonClass.CreateType<IAnonymousClassWithInterfaceExtensionTestInterface>();

            Assert.IsTrue(anonClass.ADouble == newObject.ADouble);
            Assert.IsTrue(anonClass.AnyInt == newObject.AnyInt);
            Assert.IsTrue(anonClass.TheString == newObject.TheString);
        }

        [TestMethod]
        public void Type_Creation_With_Method_In_Interface_Exception_Test()
        {
            try
            {
                var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
                var newObject = anonClass.CreateType<IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod>();

                newObject.GetAnyStringMethod(99, 'p');

                Assert.IsTrue(false);
            }
            catch (NotImplementedException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Type_Creation_With_Method_In_Interface_Test()
        {
            var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
            var newObject = anonClass.CreateType<IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod>();

            Assert.IsTrue(anonClass.ADouble == newObject.ADouble);
            Assert.IsTrue(anonClass.AnyInt == newObject.AnyInt);
            Assert.IsTrue(anonClass.TheString == newObject.TheString);
        }
    }

    public class GenericType<T>
    {
        public T Value { get; set; }
    }

    public class OuterClass
    {
        public class NestedClass
        {
            public IAnonymousClassWithInterfaceExtensionTestInterface GetValues()
            {
                return new { ADouble = 6666.666, AnyInt = 666, TheString = "TestString" }.CreateType<IAnonymousClassWithInterfaceExtensionTestInterface>();
            }
        }
    }
}