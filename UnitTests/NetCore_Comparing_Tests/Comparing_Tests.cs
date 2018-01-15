using Cauldron.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NetCore_Comparing_Tests
{
    [TestClass]
    public class Comparing_Tests
    {
        [TestMethod]
        public void Equals_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.Equals(33, 33));
            Assert.IsTrue(Comparer.Equals(33L, 33L));
            Assert.IsTrue(Comparer.Equals(33L, 33));
            Assert.IsTrue(Comparer.Equals('a', 'a'));
            Assert.IsTrue(Comparer.Equals('a', (int)'a'));
            Assert.IsTrue(Comparer.Equals('a', (byte)'a'));
            Assert.IsTrue(Comparer.Equals(TimeSpan.FromHours(22), TimeSpan.FromHours(22)));
            Assert.IsTrue(Comparer.Equals((ComparingTestClass)88, (ComparingTestClass)88));
            Assert.IsFalse(Comparer.Equals((ComparingClassWithoutOperatorAndEqualOverride)88, (ComparingClassWithoutOperatorAndEqualOverride)66));

            Assert.IsFalse(Comparer.Equals(33, null));
            Assert.IsFalse(Comparer.Equals(null, 22));

            Assert.IsTrue(Comparer.Equals(null, null));
            Assert.IsTrue(Comparer.Equals(testObject, testObject));
        }

        [TestMethod]
        public void GreaterOrEqualThan_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.GreaterThanOrEqual(33, 31));
            Assert.IsTrue(Comparer.GreaterThanOrEqual(33L, 31L));
            Assert.IsTrue(Comparer.GreaterThanOrEqual('g', 'a'));
            Assert.IsTrue(Comparer.GreaterThanOrEqual(TimeSpan.FromHours(22), TimeSpan.FromHours(21)));
            Assert.IsTrue(Comparer.GreaterThanOrEqual((ComparingTestClass)88, (ComparingTestClass)22));

            Assert.IsTrue(Comparer.GreaterThanOrEqual(33, 33));
            Assert.IsTrue(Comparer.GreaterThanOrEqual(33L, 33L));
            Assert.IsTrue(Comparer.GreaterThanOrEqual('g', 'g'));
            Assert.IsTrue(Comparer.GreaterThanOrEqual(TimeSpan.FromHours(22), TimeSpan.FromHours(22)));
            Assert.IsTrue(Comparer.GreaterThanOrEqual((ComparingTestClass)88, (ComparingTestClass)88));

            Assert.IsFalse(Comparer.GreaterThanOrEqual(33, null));
            Assert.IsFalse(Comparer.GreaterThanOrEqual(null, 22));

            Assert.IsTrue(Comparer.GreaterThanOrEqual(null, null));
            Assert.IsTrue(Comparer.GreaterThanOrEqual(testObject, testObject));
        }

        [TestMethod]
        public void GreaterThan_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.GreaterThan(33, 31));
            Assert.IsTrue(Comparer.GreaterThan(33L, 31L));
            Assert.IsTrue(Comparer.GreaterThan('g', 'a'));
            Assert.IsTrue(Comparer.GreaterThan(TimeSpan.FromHours(22), TimeSpan.FromHours(21)));
            Assert.IsTrue(Comparer.GreaterThan((ComparingTestClass)88, (ComparingTestClass)22));

            Assert.IsFalse(Comparer.GreaterThan(33, null));
            Assert.IsFalse(Comparer.GreaterThan(null, 22));
            Assert.IsFalse(Comparer.GreaterThan(null, null));
            Assert.IsFalse(Comparer.GreaterThan(testObject, testObject));
        }

        [TestMethod]
        public void InEquals_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.UnEquals(33, 24));
            Assert.IsTrue(Comparer.UnEquals(33L, 24L));
            Assert.IsTrue(Comparer.UnEquals(33L, 339));
            Assert.IsTrue(Comparer.UnEquals('a', 'v'));
            Assert.IsTrue(Comparer.UnEquals('a', (int)'v'));
            Assert.IsTrue(Comparer.UnEquals('a', (byte)'v'));
            Assert.IsTrue(Comparer.UnEquals(TimeSpan.FromHours(22), TimeSpan.FromHours(33)));
            Assert.IsTrue(Comparer.UnEquals((ComparingTestClass)88, (ComparingTestClass)66));
            Assert.IsTrue(Comparer.UnEquals((ComparingClassWithoutOperatorAndEqualOverride)88, (ComparingClassWithoutOperatorAndEqualOverride)66));

            Assert.IsTrue(Comparer.UnEquals(33, null));
            Assert.IsTrue(Comparer.UnEquals(null, 22));

            Assert.IsFalse(Comparer.UnEquals(null, null));
            Assert.IsFalse(Comparer.UnEquals(testObject, testObject));
        }

        [TestMethod]
        public void LessThan_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.LessThan(22, 31));
            Assert.IsTrue(Comparer.LessThan(9L, 31L));
            Assert.IsTrue(Comparer.LessThan('a', 'g'));
            Assert.IsTrue(Comparer.LessThan(TimeSpan.FromHours(10), TimeSpan.FromHours(21)));
            Assert.IsTrue(Comparer.LessThan((ComparingTestClass)8, (ComparingTestClass)22));

            Assert.IsFalse(Comparer.LessThan(33, null));
            Assert.IsFalse(Comparer.LessThan(null, 22));
            Assert.IsFalse(Comparer.LessThan(null, null));
            Assert.IsFalse(Comparer.LessThan(testObject, testObject));
        }

        [TestMethod]
        public void LessThanOrEqual_Tests()
        {
            var testObject = new ComparingTestClass();

            Assert.IsTrue(Comparer.LessThanOrEqual(22, 31));
            Assert.IsTrue(Comparer.LessThanOrEqual(22L, 31L));
            Assert.IsTrue(Comparer.LessThanOrEqual('g', 'z'));
            Assert.IsTrue(Comparer.LessThanOrEqual(TimeSpan.FromHours(12), TimeSpan.FromHours(21)));
            Assert.IsTrue(Comparer.LessThanOrEqual((ComparingTestClass)2, (ComparingTestClass)22));

            Assert.IsTrue(Comparer.LessThanOrEqual(33, 33));
            Assert.IsTrue(Comparer.LessThanOrEqual(33L, 33L));
            Assert.IsTrue(Comparer.LessThanOrEqual('g', 'g'));
            Assert.IsTrue(Comparer.LessThanOrEqual(TimeSpan.FromHours(22), TimeSpan.FromHours(22)));
            Assert.IsTrue(Comparer.LessThanOrEqual((ComparingTestClass)88, (ComparingTestClass)88));

            Assert.IsFalse(Comparer.LessThanOrEqual(33, null));
            Assert.IsFalse(Comparer.LessThanOrEqual(null, 22));

            Assert.IsTrue(Comparer.LessThanOrEqual(null, null));
            Assert.IsTrue(Comparer.LessThanOrEqual(testObject, testObject));
        }
    }

    #region Test Class

    internal class ComparingClassWithoutOperatorAndEqualOverride
    {
        public int value;

        public static implicit operator ComparingClassWithoutOperatorAndEqualOverride(int value) => new ComparingClassWithoutOperatorAndEqualOverride { value = value };

        public bool Equals() => true;
    }

    internal class ComparingTestClass
    {
        public int value;

        public static implicit operator ComparingTestClass(int value) => new ComparingTestClass { value = value };

        public static bool operator !=(ComparingTestClass a, ComparingTestClass b) => a.value != b.value;

        public static bool operator <(ComparingTestClass a, ComparingTestClass b) => a.value < b.value;

        public static bool operator <=(ComparingTestClass a, ComparingTestClass b) => a.value <= b.value;

        public static bool operator ==(ComparingTestClass a, ComparingTestClass b) => a.value == b.value;

        public static bool operator >(ComparingTestClass a, ComparingTestClass b) => a.value > b.value;

        public static bool operator >=(ComparingTestClass a, ComparingTestClass b) => a.value >= b.value;

        public override bool Equals(object obj) => this.Equals(obj);

        public override int GetHashCode() => this.value.GetHashCode();
    }

    #endregion Test Class
}