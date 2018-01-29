using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Extensions;
using System;

namespace Win32_Cecilator_Scriptable
{
    public class IfElseTests
    {
        public const string TestClassAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
        public const string TestMethodAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";

        private static Method assertAreEqual = null;
        private static Method assertIsTrue = null;

        private static Field testField1 = null;
        private static Field testField2 = null;
        private static Field testField3 = null;
        private static Field testField4 = null;
        private static BuilderType testType = null;

        static IfElseTests()
        {
            var type = Builder.Current.GetType("Microsoft.VisualStudio.TestTools.UnitTesting.Assert");
            assertAreEqual = type.GetMethod("AreEqual", true, new string[] { "T", "T" });
            assertIsTrue = type.GetMethod("IsTrue", true, new Type[] { typeof(bool) });
            testType = Builder.Current.CreateType("GeneratedTests", typeof(IfElseTests).Name);
            testType.CustomAttributes.Add(Builder.Current.GetType(TestClassAttribute));
            testType.CreateConstructor();
            testField1 = testType.CreateField(Modifiers.Internal, typeof(int), "testIntField_1");
            testField2 = testType.CreateField(Modifiers.Internal, typeof(int), "testIntField_2");
            testField3 = testType.CreateField(Modifiers.Internal, typeof(object), "testIntField_3");
            testField4 = testType.CreateField(Modifiers.Internal, typeof(bool), "testIntField_4");
        }

        public static void If_Field_Equal_Not_Equal(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Equal_Not_Equal), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField1).Set(22)
                .Load(testField2).Set(22)
                .If(x => x.Load(testField1).EqualsTo(testField2),
                    x => x.Load(testField2).Set(44))
                .If(x => x.Load(testField1).NotEqualsTo(testField2),
                    x => x.Load(testField1).Set(45))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 44, testField2)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 45, testField1)
                .Return()
                .Replace();
        }

        public static void If_Field_Inverted_IsTrue_IsFalse(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Inverted_IsTrue_IsFalse), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .If(x => x.Load(testField4).Invert().IsTrue(),
                    x => x.Load(testField4).Set(true))
                .Call(assertIsTrue, testField4)
                .If(x => x.Load(testField4).Invert().IsFalse(),
                    x => x.Load(testField4).Set(false))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), false, testField4)
                .Return()
                .Replace();
        }

        public static void If_Field_Inverted_IsTrue_IsFalse_WithEquals(Builder builder)
        {
            // This should actually generate the same code like If_Field_Inverted_IsTrue_IsFalse

            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Inverted_IsTrue_IsFalse_WithEquals), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .If(x => x.Load(testField4).Invert().EqualsTo(true),
                    x => x.Load(testField4).Set(true))
                .Call(assertIsTrue, testField4)
                .If(x => x.Load(testField4).Invert().NotEqualsTo(true),
                    x => x.Load(testField4).Set(false))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), false, testField4)
                .Return()
                .Replace();
        }

        public static void If_Field_Is(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Is), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField3).Set(99.8)
                .If(x => x.Load(testField3).Is(typeof(double)),
                    x => x.Call(assertIsTrue, true),
                    @else => @else.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_IsNull_IsNotNull(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_IsNull_IsNotNull), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var coder = method.NewCoder();
            coder
                .If(x => x.Load(testField3).IsNull(),
                    x => x.Load(testField3).Set(55))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 55, coder.NewCoder().Load(testField3).As(typeof(int).ToBuilderType()).ToCodeBlock())
                .If(x => x.Load(testField3).IsNotNull(),
                    x => x.Load(testField3).Set(66))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 66, coder.NewCoder().Load(testField3).As(typeof(int).ToBuilderType()).ToCodeBlock())
                .Return()
                .Replace();
        }

        public static void If_Field_IsTrue_IsFalse(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_IsTrue_IsFalse), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .If(x => x.Load(testField4).IsFalse(),
                    x => x.Load(testField4).Set(true))
                .Call(assertIsTrue, testField4)
                .If(x => x.Load(testField4).IsTrue(),
                    x => x.Load(testField4).Set(false))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), false, testField4)
                .Return()
                .Replace();
        }

        public static void If_Field_Negated(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Negated), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField1).Set(20)
                .If(x => x.Load(testField1).Negate().EqualsTo(-20),
                    x => x.Call(assertIsTrue, true),
                    @else => @else.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        private static void Implement2(Builder builder)
        {
            var type = builder.CreateType("Test", "IfElse");
            var field1 = type.CreateField(Modifiers.Public, typeof(int), "testField1");
            var field2 = type.CreateField(Modifiers.Public, typeof(int), "testField2");

            type.CustomAttributes.Add(builder.GetType(TestClassAttribute));
            type.CreateConstructor();

            var method1 = type.CreateMethod(Modifiers.Public, typeof(int), "IfElseFieldTest_Simple");
            method1.NewCoder()
                .If(x => x.Load(field1).EqualsTo(field2),
                    x => x.Load(field1).Return())
            .Load(42).Return()
            .Replace();

            var method3 = type.CreateMethod(Modifiers.Private, type, "ReturnsObject");
            method3.NewCoder().ReturnDefault().Replace();

            var method4 = type.CreateMethod(Modifiers.Private, typeof(bool), "ReturnsBoolean");
            method4.NewCoder().ReturnDefault().Replace();

            var testMethod = type.CreateMethod(Modifiers.Public, "IfElseCallTest_Simple", Type.EmptyTypes);
            testMethod.CustomAttributes.Add(builder.GetType(TestMethodAttribute));
            testMethod.NewCoder()
                    .If(x => x.Call(method1).EqualsTo(field1), x => x.Return())
                    .If(x => x.Call(method1).NotEqualsTo(int.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).EqualsTo(uint.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).EqualsTo(uint.MinValue), x => x.Return())
                    .If(x => x.Call(method1).EqualsTo((double)43843.99347), x => x.Return())
                    .If(x => x.Call(method1).EqualsTo(ulong.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).NotEqualsTo(ulong.MinValue), x => x.Return())
                    .If(x => x.Call(method1).EqualsTo(y => y.Call(method4)), x => x.Return())
                    .If(x => x.Call(method3).Is(typeof(int)), x => x.Return())
                    .If(x => x.Call(method3).IsNull(), x => x.Return())
                    .If(x => x.Call(method3).IsNotNull(), x => x.Return())
                    .If(x => x.Call(method1).Invert().IsTrue(), x => x.Return())
                    .If(x => x.Call(method4).IsFalse(), x => x.Return())
                    .If(x => x.Call(method4).Invert().IsTrue(), x => x.Return())
                    .If(x => x.Call(method4).IsTrue(), x => x.Return())
                    .If(x => x.Call(method3).Call(method4).IsTrue(), x => x.Return())
                    .Return()
                    .Replace();
        }
    }
}