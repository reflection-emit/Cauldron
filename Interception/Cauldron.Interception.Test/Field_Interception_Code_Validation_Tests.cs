using Cauldron.Interception.Test.Interceptors;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

#endif

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Field_Interception_Code_Validation_Tests
    {
        [TestPropertyInterceptor]
        private static double fieldTwo = 0.3;

        [TestPropertyInterceptor]
        private long fieldOne;

        [TestPropertyInterceptor]
        private int? nullableValueType;

        [TestMethod]
        public void Instance_Method_Field_Interception()
        {
            this.fieldOne = 50;
            Assert.AreEqual(50, this.fieldOne);

            this.fieldOne = 30;
            Assert.AreEqual(9999, this.fieldOne);
        }

        [TestMethod]
        public void Instance_Nullable_Method_Field_Interception()
        {
            this.nullableValueType = 50;
            Assert.AreEqual(50, this.nullableValueType);

            this.nullableValueType = 30;
            Assert.AreEqual(30, this.nullableValueType);
        }

        [TestMethod]
        public void Static_Method_Field_Interception()
        {
            fieldTwo = 4.6;
            Assert.AreEqual(4.6, fieldTwo);

            fieldTwo = 66;
            Assert.AreEqual(78344.796875, fieldTwo);
        }

        [TestMethod]
        public void Private_Field_Accessed_By_Nested_Type()
        {
            var parent = new ClassWithNestedType();
            var child = new ClassWithNestedType.NestedTypeAccessingPrivateField(parent);

            Assert.AreEqual(66, parent.AField);
        }

        //[TestMethod]
        //public void Generic_Type_With_Generic_Field()
        //{
        //    var generic = new GenericTypeWithGenericField<long>();

        //    generic.FieldTwo = 66;
        //    Assert.AreEqual(99, generic.FieldTwo);
        //}

        [TestMethod]
        public void Generic_Type_With_Field()
        {
            var generic = new GenericTypeWithField<long>();

            generic.FieldTwo = 66;
            Assert.AreEqual(99, generic.FieldTwo);
        }
    }

    //public class GenericTypeWithGenericField<T>
    //{
    //    [TestPropertyInterceptor]
    //    private T fieldTwo;

    //    public T FieldTwo
    //    {
    //        get { return fieldTwo; }
    //        set { fieldTwo = value; }
    //    }
    //}

    public class GenericTypeWithField<T>
    {
        [TestPropertyInterceptor]
        private long fieldTwo;

        private int secondField;

        public long FieldTwo
        {
            get { return fieldTwo; }
            set { fieldTwo = value; }
        }

        private void bla(object value)
        {
            this.secondField = Convert.ToInt32(value);
        }
    }

    public class ClassWithNestedType
    {
        [TestPropertyInterceptor]
        private long afield;

        public class NestedTypeAccessingPrivateField
        {
            private ClassWithNestedType parent;

            public NestedTypeAccessingPrivateField(ClassWithNestedType parent)
            {
                this.parent = parent;
                this.parent.afield = 66;
            }
        }

        public long AField { get { return afield; } }
    }
}