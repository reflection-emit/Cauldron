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
        private static Field testField5 = null;
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
            testField5 = testType.CreateField(Modifiers.Internal, testType, "testIntField_5");
        }

        public static void If_Field_AndAnd_Field(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_AndAnd_Field), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField4, true)
                .SetValue(testField1, 22)
                .If(x => x.Load(testField4).Is(true).AndAnd(y => y.Load(testField1).Is(true)),
                    x => x.Call(assertIsTrue, true),
                    @else => @else.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_Call_Bool(Builder builder)
        {
            var name = nameof(If_Field_Call_Bool);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var methodToBeCalled = testType.CreateMethod(Modifiers.Internal, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            methodToBeCalled.NewCoder()
                .Load(33)
                .Return()
                .Replace();

            method.NewCoder()
                .SetValue(testField5, x => x.NewObj(testType.ParameterlessContructor))
                .If(x => x.Load(testField5).Call(methodToBeCalled).Is(33),
                    x => x.Call(assertIsTrue, true),
                    x => x.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_Call_Int(Builder builder)
        {
            var name = nameof(If_Field_Call_Int);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var methodToBeCalled = testType.CreateMethod(Modifiers.Internal, typeof(bool), name + "_TestMethod", Type.EmptyTypes);
            methodToBeCalled.NewCoder()
                .Load(true)
                .Return()
                .Replace();

            method.NewCoder()
                .SetValue(testField5, x => x.NewObj(testType.ParameterlessContructor))
                .If(x => x.Load(testField5).Call(methodToBeCalled).Is(true),
                    x => x.Call(assertIsTrue, true),
                    x => x.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_Call_Object(Builder builder)
        {
            var name = nameof(If_Field_Call_Object);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var methodToBeCalled = testType.CreateMethod(Modifiers.Internal, typeof(object), name + "_TestMethod", Type.EmptyTypes);
            methodToBeCalled.NewCoder()
                .NewObj(testType.ParameterlessContructor)
                .Return()
                .Replace();

            method.NewCoder()
                .SetValue(testField3, x => x.NewObj(testType.ParameterlessContructor))
                .If(x => x.Load(testField3).As(testType).Call(methodToBeCalled).Is(testType),
                    x => x.Call(assertIsTrue, true),
                    x => x.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_Equal_Not_Equal(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Equal_Not_Equal), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField1, 22)
                .SetValue(testField2, 22)
                .If(x => x.Load(testField1).Is(testField2),
                    x => x.SetValue(testField2, 44))
                .If(x => x.Load(testField1).IsNot(testField2),
                    x => x.SetValue(testField1, 45))
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
                .If(x => x.Load(testField4).Invert().Is(true),
                    x => x.SetValue(testField4, true))
                .Call(assertIsTrue, testField4)
                .End
                .If(x => x.Load(testField4).Invert().Is(false),
                    x => x.SetValue(testField4, false))
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
                .If(x => x.Load(testField4).Invert().Is(true),
                    x => x.SetValue(testField4, true))
                .Call(assertIsTrue, testField4)
                .End
                .If(x => x.Load(testField4).Invert().IsNot(true),
                    x => x.SetValue(testField4, false))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), false, testField4)
                .Return()
                .Replace();
        }

        public static void If_Field_Is(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Is), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField3, 99.8)
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
                    x => x.SetValue(testField3, 55))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 55, coder.NewCoder().Load(testField3).As(typeof(int).ToBuilderType()))
                .End
                .If(x => x.Load(testField3).IsNotNull(),
                    x => x.SetValue(testField3, 66))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), 66, coder.NewCoder().Load(testField3).As(typeof(int).ToBuilderType()))
                .Return()
                .Replace();
        }

        public static void If_Field_IsTrue_IsFalse(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_IsTrue_IsFalse), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .If(x => x.Load(testField4).Is(false),
                    x => x.SetValue(testField4, true))
                .Call(assertIsTrue, testField4)
                .End
                .If(x => x.Load(testField4).Is(true),
                    x => x.SetValue(testField4, false))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), false, testField4)
                .Return()
                .Replace();
        }

        public static void If_Field_Negated(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_Negated), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField1, 20)
                .If(x => x.Load(testField1).Negate().Is(-20),
                    x => x.Call(assertIsTrue, true),
                    @else => @else.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void If_Field_OrOr_Field(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(If_Field_OrOr_Field), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField4, true)
                .SetValue(testField1, 22)
                .SetValue(testField3, null)
                .If(x => x.Load(testField4).Is(true).OrOr(testField1).OrOr(y => y.Load(testField3).IsNull()),
                    x => x.Call(assertIsTrue, true),
                    @else => @else.Call(assertIsTrue, false))
                .Return()
                .Replace();
        }

        public static void Implement2(Builder builder)
        {
            var type = builder.CreateType("Test", "IfElse");
            var field1 = type.CreateField(Modifiers.Public, typeof(int), "testField1");
            var field2 = type.CreateField(Modifiers.Public, typeof(int), "testField2");

            type.CustomAttributes.Add(builder.GetType(TestClassAttribute));
            type.CreateConstructor();

            var method1 = type.CreateMethod(Modifiers.Public, typeof(int), "IfElseFieldTest_Simple");
            method1.NewCoder()
                .If(x => x.Load(field1).Is(field2),
                    x => x.Load(field1).Return())
            .Load(42).Return()
            .Replace();

            var method3 = type.CreateMethod(Modifiers.Private, type, "ReturnsObject");
            method3.NewCoder().DefaultValue().Return().Replace();

            var method4 = type.CreateMethod(Modifiers.Private, typeof(bool), "ReturnsBoolean");
            method4.NewCoder().DefaultValue().Return().Replace();

            var testMethod = type.CreateMethod(Modifiers.Public, "IfElseCallTest_Simple", Type.EmptyTypes);
            testMethod.CustomAttributes.Add(builder.GetType(TestMethodAttribute));
            testMethod.NewCoder()
                    .If(x => x.Call(method1).Is(field1), x => x.Return())
                    .If(x => x.Call(method1).IsNot(int.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).Is(uint.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).Is(uint.MinValue), x => x.Return())
                    .If(x => x.Call(method1).Is((double)43843.99347), x => x.Return())
                    .If(x => x.Call(method1).Is(ulong.MaxValue), x => x.Return())
                    .If(x => x.Call(method1).IsNot(ulong.MinValue), x => x.Return())
                    .If(x => x.Call(method1).Is(y => y.Call(method4)), x => x.Return())
                    .If(x => x.Call(method3).Is(typeof(int)), x => x.Return())
                    .If(x => x.Call(method3).IsNull(), x => x.Return())
                    .If(x => x.Call(method3).IsNotNull(), x => x.Return())
                    .If(x => x.Call(method1).Invert().Is(true), x => x.Return())
                    .If(x => x.Call(method4).Is(false), x => x.Return())
                    .If(x => x.Call(method4).Invert().Is(true), x => x.Return())
                    .If(x => x.Call(method4).Is(true), x => x.Return())
                    .If(x => x.Call(method3).Call(method4).Is(true), x => x.Return())
                    .Return()
                    .Replace();
        }
    }
}