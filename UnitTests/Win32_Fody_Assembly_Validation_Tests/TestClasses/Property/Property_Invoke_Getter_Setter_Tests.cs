using Cauldron.UnitTest.Interceptors.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.UnitTest.AssemblyValidation.TestClasses.Property
{
    [TestClass]
    public class Property_Invoke_Getter_Setter_Tests : Property_InvokeTest_Base
    {
        #region Long property invokation tests

        [Property_Getter_Setter]
        public long Long_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public long Long_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public long Long_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Long_Property_Exception_Test()
        {
            this.AssertException(() => this.Long_Property_Exception = long.MaxValue);
            this.AssertException(() => this.Long_Property_Exception);
        }

        [TestMethod]
        public void Long_Property_Getter_Test()
        {
            var value = this.Long_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Long_Property_Setter_Test()
        {
            this.Long_Property = long.MaxValue;
            this.AssertSetter();
        }

        [TestMethod]
        public void Long_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Long_Property_Suppressed_Setter, x => this.Long_Property_Suppressed_Setter = x, long.MaxValue);
        }

        #endregion Long property invokation tests

        #region String property invokation tests

        [Property_Getter_Setter]
        public string String_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public string String_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public string String_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void String_Property_Exception_Test()
        {
            this.AssertException(() => this.String_Property_Exception = "value");
            this.AssertException(() => this.String_Property_Exception);
        }

        [TestMethod]
        public void String_Property_Getter_Test()
        {
            var value = this.String_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void String_Property_Setter_Test()
        {
            this.String_Property = "value";
            this.AssertSetter();
        }

        [TestMethod]
        public void String_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.String_Property_Suppressed_Setter, x => this.String_Property_Suppressed_Setter = x, "value");
        }

        #endregion String property invokation tests

        #region Guid property invokation tests

        [Property_Getter_Setter]
        public Guid Guid_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public Guid Guid_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public Guid Guid_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Guid_Property_Exception_Test()
        {
            this.AssertException(() => this.Guid_Property_Exception = Guid.NewGuid());
            this.AssertException(() => this.Guid_Property_Exception);
        }

        [TestMethod]
        public void Guid_Property_Getter_Test()
        {
            var value = this.Guid_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Guid_Property_Setter_Test()
        {
            this.Guid_Property = Guid.NewGuid();
            this.AssertSetter();
        }

        [TestMethod]
        public void Guid_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Guid_Property_Suppressed_Setter, x => this.Guid_Property_Suppressed_Setter = x, Guid.NewGuid());
        }

        #endregion Guid property invokation tests

        #region Integer property invokation tests

        [Property_Getter_Setter]
        public int Integer_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public int Integer_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public int Integer_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Integer_Property_Exception_Test()
        {
            this.AssertException(() => this.Integer_Property_Exception = 68755);
            this.AssertException(() => this.Integer_Property_Exception);
        }

        [TestMethod]
        public void Integer_Property_Getter_Test()
        {
            var value = this.Integer_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Integer_Property_Setter_Test()
        {
            this.Integer_Property = 68755;
            this.AssertSetter();
        }

        [TestMethod]
        public void Integer_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Integer_Property_Suppressed_Setter, x => this.Integer_Property_Suppressed_Setter = x, 68755);
        }

        #endregion Integer property invokation tests

        #region Double property invokation tests

        [Property_Getter_Setter]
        public double Double_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public double Double_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public double Double_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Double_Property_Exception_Test()
        {
            this.AssertException(() => this.Double_Property_Exception = 32432.332);
            this.AssertException(() => this.Double_Property_Exception);
        }

        [TestMethod]
        public void Double_Property_Getter_Test()
        {
            var value = this.Double_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Double_Property_Setter_Test()
        {
            this.Double_Property = 32432.332;
            this.AssertSetter();
        }

        [TestMethod]
        public void Double_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Double_Property_Suppressed_Setter, x => this.Double_Property_Suppressed_Setter = x, 32432.332);
        }

        #endregion Double property invokation tests

        #region NullableInteger property invokation tests

        [Property_Getter_Setter]
        public int? NullableInteger_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public int? NullableInteger_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public int? NullableInteger_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void NullableInteger_Property_Exception_Test()
        {
            this.AssertException(() => this.NullableInteger_Property_Exception = 42433);
            this.AssertException(() => this.NullableInteger_Property_Exception);
        }

        [TestMethod]
        public void NullableInteger_Property_Getter_Test()
        {
            var value = this.NullableInteger_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void NullableInteger_Property_Setter_Test()
        {
            this.NullableInteger_Property = 42433;
            this.AssertSetter();
        }

        [TestMethod]
        public void NullableInteger_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.NullableInteger_Property_Suppressed_Setter, x => this.NullableInteger_Property_Suppressed_Setter = x, 42433);
        }

        #endregion NullableInteger property invokation tests

        #region ListOfStrings property invokation tests

        [Property_Getter_Setter]
        public List<string> ListOfStrings_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public List<string> ListOfStrings_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public List<string> ListOfStrings_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void ListOfStrings_Property_Exception_Test()
        {
            this.AssertException(() => this.ListOfStrings_Property_Exception = new List<string> { "This", "is", "a", "string" });
            this.AssertException(() => this.ListOfStrings_Property_Exception);
        }

        [TestMethod]
        public void ListOfStrings_Property_Getter_Test()
        {
            var value = this.ListOfStrings_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void ListOfStrings_Property_Setter_Test()
        {
            this.ListOfStrings_Property = new List<string> { "This", "is", "a", "string" };
            this.AssertSetter();
        }

        [TestMethod]
        public void ListOfStrings_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.ListOfStrings_Property_Suppressed_Setter, x => this.ListOfStrings_Property_Suppressed_Setter = x, new List<string> { "This", "is", "a", "string" });
        }

        #endregion ListOfStrings property invokation tests

        #region IEnumerable property invokation tests

        [Property_Getter_Setter]
        public IEnumerable IEnumerable_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public IEnumerable IEnumerable_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public IEnumerable IEnumerable_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void IEnumerable_Property_Exception_Test()
        {
            this.AssertException(() => this.IEnumerable_Property_Exception = new string[] { "This", "is", "a", "string" });
            this.AssertException(() => this.IEnumerable_Property_Exception);
        }

        [TestMethod]
        public void IEnumerable_Property_Getter_Test()
        {
            var value = this.IEnumerable_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void IEnumerable_Property_Setter_Test()
        {
            this.IEnumerable_Property = new string[] { "This", "is", "a", "string" };
            this.AssertSetter();
        }

        [TestMethod]
        public void IEnumerable_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.IEnumerable_Property_Suppressed_Setter, x => this.IEnumerable_Property_Suppressed_Setter = x, new string[] { "This", "is", "a", "string" });
        }

        #endregion IEnumerable property invokation tests

        #region Decimal property invokation tests

        [Property_Getter_Setter]
        public decimal Decimal_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public decimal Decimal_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public decimal Decimal_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Decimal_Property_Exception_Test()
        {
            this.AssertException(() => this.Decimal_Property_Exception = (decimal)2342.343);
            this.AssertException(() => this.Decimal_Property_Exception);
        }

        [TestMethod]
        public void Decimal_Property_Getter_Test()
        {
            var value = this.Decimal_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Decimal_Property_Setter_Test()
        {
            this.Decimal_Property = (decimal)2342.343;
            this.AssertSetter();
        }

        [TestMethod]
        public void Decimal_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Decimal_Property_Suppressed_Setter, x => this.Decimal_Property_Suppressed_Setter = x, (decimal)2342.343);
        }

        #endregion Decimal property invokation tests

        #region Class property invokation tests

        [Property_Getter_Setter]
        public Property_Invoke_Getter_Setter_Tests Class_Property { get; set; }

        [Property_Getter_Setter_Exception]
        public Property_Invoke_Getter_Setter_Tests Class_Property_Exception { get; set; }

        [Property_Getter_Setter_NoSet]
        public Property_Invoke_Getter_Setter_Tests Class_Property_Suppressed_Setter { get; set; }

        [TestMethod]
        public void Class_Property_Exception_Test()
        {
            this.AssertException(() => this.Class_Property_Exception = new Property_Invoke_Getter_Setter_Tests());
            this.AssertException(() => this.Class_Property_Exception);
        }

        [TestMethod]
        public void Class_Property_Getter_Test()
        {
            var value = this.Class_Property;
            this.AssertGetter();
        }

        [TestMethod]
        public void Class_Property_Setter_Test()
        {
            this.Class_Property = new Property_Invoke_Getter_Setter_Tests();
            this.AssertSetter();
        }

        [TestMethod]
        public void Class_Property_Suppessed_Setter_Test()
        {
            this.AssertNoSetter(() => this.Class_Property_Suppressed_Setter, x => this.Class_Property_Suppressed_Setter = x, new Property_Invoke_Getter_Setter_Tests());
        }

        #endregion Class property invokation tests
    }
}