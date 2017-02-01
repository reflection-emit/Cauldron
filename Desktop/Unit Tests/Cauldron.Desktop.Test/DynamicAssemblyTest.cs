using Cauldron.Core.Extensions;
using Cauldron.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection.Emit;

namespace Cauldron.Test
{
    public interface DynamicAssemblyTest_TestInterface
    {
        string TestMethod();
    }

    [TestClass]
    public class DynamicAssemblyTest
    {
        [TestMethod]
        public void Create_Type_With_Method()
        {
            Type type = null;

            using (var builder = DynamicAssembly.CreateBuilder("Test_Type"))
            {
                builder.Implement("TestMethod", x => x.ToString());
                type = builder;
            }

            var obj = type.CreateInstance();
            var method = obj.GetType().GetMethod("TestMethod");
            var result = method.Invoke(obj, new object[0]) as string;

            Assert.AreEqual(type.Name, result);
        }

        [TestMethod]
        public void Create_Type_With_Method_From_Class()
        {
            Type type = null;

            using (var builder = DynamicAssembly.CreateBuilder<DynamicAssemblyTest_TestClass>())
            {
                builder.Implement("TestMethod", x => x.MethodInClass());
                type = builder;
            }

            var obj = type.CreateInstance();
            var method = obj.GetType().GetMethod("TestMethod");
            var result = method.Invoke(obj, new object[0]) as string;

            Assert.AreEqual("DynamicAssemblyTest", result);
        }

        [TestMethod]
        public void Create_Type_With_Method_From_Class_Override()
        {
            Type type = null;

            using (var builder = DynamicAssembly.CreateBuilder<DynamicAssemblyTest_TestClass>())
            {
                builder.Implement<int, int>("VeryGoodMethod", (x, t1) => t1 * 10);
                type = builder;
            }

            var obj = type.CreateInstance() as DynamicAssemblyTest_TestClass;
            Assert.AreEqual(100, obj.VeryGoodMethod(10));
            Assert.AreEqual(20, obj.VeryGoodMethod(2));
            Assert.AreEqual(1020, obj.VeryGoodMethod(102));
            Assert.AreEqual(500, obj.VeryGoodMethod(50));
        }

        [TestMethod]
        public void Create_Type_With_Method_From_Interface()
        {
            Type type = null;

            using (var builder = DynamicAssembly.CreateBuilder<DynamicAssemblyTest_TestInterface>())
            {
                builder.Implement("TestMethod", x => typeof(DynamicAssemblyTest_TestInterface).Name);
                type = builder;
            }

            var obj = type.CreateInstance() as DynamicAssemblyTest_TestInterface;
            Assert.AreEqual("DynamicAssemblyTest_TestInterface", obj.TestMethod());
        }

        [TestMethod]
        public void Create_Type_With_Method_From_Scratch()
        {
            Type type = null;

            using (var builder = DynamicAssembly.CreateBuilder("Test_Type"))
            {
                builder.Implement("TestMethod", x =>
                {
                    var localVar = x.DeclareLocal(typeof(Int32));

                    x.Emit(OpCodes.Ldarg_1); // Get the parameters
                    x.Emit(OpCodes.Ldc_I4_2); // Load int 2 to stack
                    x.Emit(OpCodes.Mul); // multiply
                    x.Emit(OpCodes.Stloc, localVar); // store last value of stack to local var
                    x.Emit(OpCodes.Ldstr, "Hello {0}"); // load to string to stack
                    x.Emit(OpCodes.Ldloc, localVar); // load local var to stack
                    x.Emit(OpCodes.Box, typeof(int)); // box last value in stack
                    x.Emit(OpCodes.Call, typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) })); // call the string.format method

                    x.Emit(OpCodes.Ret); // return the last value in stack
                }, typeof(string), new Type[] { typeof(int) });
                type = builder;
            }

            var obj = type.CreateInstance();
            var method = obj.GetType().GetMethod("TestMethod");
            var result = method.Invoke(obj, new object[] { 20 }) as string;

            Assert.AreEqual("Hello 40", result);
        }
    }

    public class DynamicAssemblyTest_TestClass
    {
        public string MethodInClass() =>
            "DynamicAssemblyTest";

        public virtual int VeryGoodMethod(int value) => value * 2;
    }
}