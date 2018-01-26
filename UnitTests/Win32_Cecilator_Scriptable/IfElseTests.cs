using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Extensions;
using System;

namespace Win32_Cecilator_Scriptable
{
    public class IfElseTests
    {
        public const string TestClassAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
        public const string TestMethodAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";

        public static void Implement(Builder builder)
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

            var testMethod = type.CreateMethod(Modifiers.Public, "IfElseCallTest_Simple", Type.EmptyTypes);
            testMethod.CustomAttributes.Add(builder.GetType(TestMethodAttribute));
            testMethod.NewCoder()
                    .If(x => x.Call(method1).EqualsTo(field1), x => x.Return())
                    .Return()
                    .If(x => x.Call(method1).EqualsTo(int.MaxValue), x => x.Return())
                    .Replace();
        }
    }
}