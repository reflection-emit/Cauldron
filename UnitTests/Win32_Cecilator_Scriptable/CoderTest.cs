using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Cecilator.Extensions;
using System;

namespace Win32_Cecilator_Scriptable
{
    public class CoderTest
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

        static CoderTest()
        {
            var type = Builder.Current.GetType("Microsoft.VisualStudio.TestTools.UnitTesting.Assert");
            assertAreEqual = type.GetMethod("AreEqual", true, new string[] { "T", "T" });
            assertIsTrue = type.GetMethod("IsTrue", true, new Type[] { typeof(bool) });
            testType = Builder.Current.CreateType("GeneratedTests", typeof(CoderTest).Name);
            testType.CustomAttributes.Add(Builder.Current.GetType(TestClassAttribute));
            testType.CreateConstructor();
            testField1 = testType.CreateField(Modifiers.Internal, typeof(int), "testIntField_1");
            testField2 = testType.CreateField(Modifiers.Internal, typeof(int), "testIntField_2");
            testField3 = testType.CreateField(Modifiers.Internal, typeof(object), "testIntField_3");
            testField4 = testType.CreateField(Modifiers.Internal, typeof(bool), "testIntField_4");
            testField5 = testType.CreateField(Modifiers.Internal, testType, "testIntField_5");
        }

        public static void Arg_BinaryOperations(Builder builder)
        {
            var name = nameof(Arg_BinaryOperations);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var method2 = testType.CreateMethod(Modifiers.Private, name + "_TestMethod", typeof(int), typeof(bool));

            method2.NewCoder()
                .SetValue(CodeBlocks.GetParameter(0), 3)
                .SetValue(CodeBlocks.GetParameter(1), true)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 15, x => x.Load(CodeBlocks.GetParameter(0)).Or(y => 4).Or(y => 9))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 2, x => x.Load(CodeBlocks.GetParameter(0)).And(y => 2))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), x => true, x => x.Load(CodeBlocks.GetParameter(1)).Or(y => 2))
                    .Return()
                    .Replace();

            method.NewCoder().Call(method2, 2000, true).Return().Replace();
        }

        public static void Arg_NewObj(Builder builder)
        {
            var name = nameof(Arg_NewObj);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var otherMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            otherMethod.NewCoder()
                .Load(10)
                    .Return()
                    .Replace();

            var fieldMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", typeof(object));
            fieldMethod.NewCoder()
                .SetValue(CodeBlocks.GetParameter(0), x => x.NewObj(testType.ParameterlessContructor))
                .SetValue(testField3, x => x.Load(CodeBlocks.GetParameter(0)))
                .Call(assertAreEqual.MakeGeneric(testType), x => x.Load(CodeBlocks.GetParameter(0)), x => x.Load(testField3))
                .End
                .Load(CodeBlocks.GetParameter(0)).As(testType).Call(otherMethod)
                    .Return()
                    .Replace();

            method.NewCoder()
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 10, x => x.Call(fieldMethod, y => y.NewObj(testType.ParameterlessContructor)))
                    .Return()
                    .Replace();
        }

        public static void Arg_Nullable(Builder builder)
        {
            var name = nameof(Arg_Nullable);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var method2 = testType.CreateMethod(Modifiers.Internal, name + "_Test_Method", typeof(int?));
            var nullableField = testType.CreateField(Modifiers.Internal, BuilderType.Nullable.MakeGeneric(BuilderType.Int32), "nullable_field");

            method2.NewCoder()
                .SetValue(CodeBlocks.GetParameter(0), x => 88)
                .Call(assertAreEqual.MakeGeneric(typeof(int?)), x => 88, x => x.Load(CodeBlocks.GetParameter(0)))
                    .Return()
                    .Replace();

            method.NewCoder()
                .Call(method2, 77)
                .Return()
                .Replace();
        }

        public static void Default_Type_Value(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Default_Type_Value), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 0, x => x.Load(CodeBlocks.DefaultValueOf(BuilderType.Int32)))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), x => false, x => x.Load(CodeBlocks.DefaultValueOf(BuilderType.Boolean)))
                .Call(assertAreEqual.MakeGeneric(testType), x => null, x => x.Load(CodeBlocks.DefaultValueOf(testType)))
                    .Return()
                    .Replace();
        }

        public static void Field_As_Call(Builder builder)
        {
            var name = nameof(Field_As_Call);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var fieldMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            fieldMethod.NewCoder()
                .Load(10)
                    .Return()
                    .Replace();

            method.NewCoder()
                    .SetValue(testField3, x => x.NewObj(testType.ParameterlessContructor))
                    .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 10, x => x.Load(testField3).As(testType).Call(fieldMethod))
                    .Return()
                    .Replace();
        }

        public static void Field_As_Load(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_As_Load), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField3, x => x.NewObj(testType.ParameterlessContructor))
                .Load(testField3).As(testType).SetValue(testField1, 466)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 466, x => x.Load(testField3).As(testType).Load(testField1))
                    .Return()
                    .Replace();
        }

        public static void Field_BinaryOperations(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_BinaryOperations), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField1, 3)
                .SetValue(testField4, true)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 15, x => x.Load(testField1).Or(y => 4).Or(y => 9))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 2, x => x.Load(testField1).And(y => 2))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), x => true, x => x.Load(testField4).Or(y => 2))
                    .Return()
                    .Replace();
        }

        public static void Field_Load_Set(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_Load_Set), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField1, 34)
                    .Call(assertAreEqual.MakeGeneric(typeof(int)), 34, testField1)
                    .Return()
                    .Replace();
        }

        public static void Field_NewObj(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_NewObj), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(testField5, x => x.NewObj(testType.ParameterlessContructor))
                .SetValue(testField3, testField5)
                .Call(assertAreEqual.MakeGeneric(testType), testField3, testField5)
                    .Return()
                    .Replace();
        }

        public static void Field_NewObj_Load_Delegate(Builder builder)
        {
            var field1Name = "field_1" + nameof(Field_NewObj_Load_Delegate);
            var field2Name = "field_2" + nameof(Field_NewObj_Load_Delegate);

            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_NewObj_Load_Delegate), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));
            method.CreateField(testType, field1Name);
            method.CreateField(BuilderType.Object, field2Name);

            method.NewCoder()
                .SetValue(x => x.GetField(field1Name), x => x.NewObj(testType.ParameterlessContructor))
                .SetValue(x => x.GetField(field2Name), x => x.Load(y => y.GetField(field1Name)))
                .Call(assertAreEqual.MakeGeneric(testType), x => x.Load(y => y.GetField(field1Name)), x => x.Load(y => y.GetField(field2Name)))
                    .Return()
                    .Replace();
        }

        public static void Field_Nullable(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_Nullable), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var nullableField = testType.CreateField(Modifiers.Internal, BuilderType.Nullable.MakeGeneric(BuilderType.Int32), "nullable_field");

            method.NewCoder()
                .SetValue(nullableField, x => 88)
                .Call(assertAreEqual.MakeGeneric(typeof(int?)), x => 88, x => x.Load(nullableField))
                    .Return()
                    .Replace();
        }

        public static void Field_Return(Builder builder)
        {
            var name = nameof(Field_Return);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var fieldMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            fieldMethod.NewCoder()
                .SetValue(testField1, 55)
                .Load(testField1)
                    .Return()
                    .Replace();

            method.NewCoder()
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 55, x => x.Call(fieldMethod))
                    .Return()
                    .Replace();
        }

        public static void Field_Set_With_Call(Builder builder)
        {
            var name = nameof(Field_Set_With_Call);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var fieldMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            fieldMethod.NewCoder()
                .Load(987)
                    .Return()
                    .Replace();

            method.NewCoder()
                .SetValue(testField1, x => x.Call(fieldMethod))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 987, x => x.Load(testField1))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_As_Call(Builder builder)
        {
            var name = nameof(LocalVariable_As_Call);

            var method = testType.CreateMethod(Modifiers.Public, name, Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var fieldMethod = testType.CreateMethod(Modifiers.Private, typeof(int), name + "_TestMethod", Type.EmptyTypes);
            fieldMethod.NewCoder()
                .Load(234)
                    .Return()
                    .Replace();

            var var1 = method.GetOrCreateVariable(typeof(object));

            method.NewCoder()
                .SetValue(var1, x => x.NewObj(testType.ParameterlessContructor))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 234, x => x.Load(var1).As(testType).Call(fieldMethod))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_As_Load(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_As_Load), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var var1 = method.GetOrCreateVariable(typeof(object));

            method.NewCoder()
                .SetValue(var1, x => x.NewObj(testType.ParameterlessContructor))
                .Load(var1).As(testType).SetValue(testField1, 466)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 466, x => x.Load(var1).As(testType).Load(testField1))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_BinaryOperations(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_BinaryOperations), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var var1 = method.GetOrCreateVariable(typeof(int));
            var var2 = method.GetOrCreateVariable(typeof(bool));

            method.NewCoder()
                .SetValue(var1, 3)
                .SetValue(var2, true)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 15, x => x.Load(var1).Or(y => 4).Or(y => 9))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 2, x => x.Load(var1).And(y => 2))
                .Call(assertAreEqual.MakeGeneric(typeof(bool)), x => true, x => x.Load(var2).Or(y => 2))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_BinaryOperations_Implicit_Overriden_And(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_BinaryOperations_Implicit_Overriden_And), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var testClassType = builder.GetType("Win32_Cecilator_Tests.TestClass");
            var var1 = method.GetOrCreateVariable(testClassType);
            var var2 = method.GetOrCreateVariable(BuilderType.Int32);

            method.NewCoder()
                .SetValue(var1, 14)
                .SetValue(var2, 9)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 15, x => x.Load(var1).Or(var2))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_NewObj(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_NewObj), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .SetValue(variable: x => x.GetOrCreateVariable(testType, "local_test_1"), value: x => x.NewObj(testType.ParameterlessContructor))
                .SetValue(variable: x => x.GetOrCreateVariable(testType, "local_test_2"), value: x => method.GetVariable("local_test_1"))
                .Call(assertAreEqual.MakeGeneric(testType), x => method.GetVariable("local_test_1"), x => method.GetVariable("local_test_2"))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_Nullable(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_Nullable), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var var1 = method.GetOrCreateVariable(BuilderType.Nullable.MakeGeneric(BuilderType.Int32));

            method.NewCoder()
                .SetValue(var1, x => 88)
                .Call(assertAreEqual.MakeGeneric(typeof(int?)), x => 88, x => x.Load(var1))
                    .Return()
                    .Replace();
        }
    }
}