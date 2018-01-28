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