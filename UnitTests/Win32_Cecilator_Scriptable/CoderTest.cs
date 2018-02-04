using Cauldron.Interception.Cecilator;
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
                .LoadArg(0).Set(x => x.NewObj(testType.ParameterlessContructor))
                .Load(testField3).Set(x => x.LoadArg(0))
                .Call(assertAreEqual.MakeGeneric(testType), x => x.LoadArg(0), x => x.Load(testField3))
                .LoadArg(0).As(testType).Call(otherMethod)
                    .Return()
                    .Replace();

            method.NewCoder()
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 10, x => x.Call(fieldMethod, y => y.NewObj(testType.ParameterlessContructor)))
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
                .Load(testField3).Set(x => x.NewObj(testType.ParameterlessContructor))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 10, x => x.Load(testField3).As(testType).Call(fieldMethod))
                    .Return()
                    .Replace();
        }

        public static void Field_As_Load(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_As_Load), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField3).Set(x => x.NewObj(testType.ParameterlessContructor))
                .Load(testField3).As(testType).Load(testField1).Set(466)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 466, x => x.Load(testField3).As(testType).Load(testField1))
                    .Return()
                    .Replace();
        }

        public static void Field_Load_Set(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_Load_Set), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField1)
                    .Set(34)
                    .Call(assertAreEqual.MakeGeneric(typeof(int)), 34, testField1)
                    .Return()
                    .Replace();
        }

        public static void Field_NewObj(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(Field_NewObj), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .Load(testField5)
                    .Set(x => x.NewObj(testType.ParameterlessContructor))
                .Load(testField3)
                    .Set(testField5)
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
                .LoadField(field1Name)
                    .Set(x => x.NewObj(testType.ParameterlessContructor))
                .LoadField(field2Name)
                    .Set(x => x.LoadField(field1Name))
                .Call(assertAreEqual.MakeGeneric(testType), x => x.LoadField(field1Name), x => x.LoadField(field2Name))
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
                .Load(testField1)
                    .Set(55)
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
                .Load(testField1).Set(x => x.Call(fieldMethod))
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

            var var1 = method.CreateVariable(typeof(object));

            method.NewCoder()
                .Load(var1).Set(x => x.NewObj(testType.ParameterlessContructor))
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 234, x => x.Load(var1).As(testType).Call(fieldMethod))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_As_Load(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_As_Load), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            var var1 = method.CreateVariable(typeof(object));

            method.NewCoder()
                .Load(var1).Set(x => x.NewObj(testType.ParameterlessContructor))
                .Load(var1).As(testType).Load(testField1).Set(466)
                .Call(assertAreEqual.MakeGeneric(typeof(int)), x => 466, x => x.Load(var1).As(testType).Load(testField1))
                    .Return()
                    .Replace();
        }

        public static void LocalVariable_NewObj(Builder builder)
        {
            var method = testType.CreateMethod(Modifiers.Public, nameof(LocalVariable_NewObj), Type.EmptyTypes);
            method.CustomAttributes.Add(builder.GetType(TestMethodAttribute));

            method.NewCoder()
                .LoadVariable(testType, "local_test_1")
                    .Set(x => x.NewObj(testType.ParameterlessContructor))
                .LoadVariable("local_test_2")
                    .Set(x => x.LoadVariable("local_test_1"))
                .Call(assertAreEqual.MakeGeneric(testType), x => x.LoadVariable("local_test_1"), x => x.LoadVariable("local_test_2"))
                    .Return()
                    .Replace();
        }
    }
}