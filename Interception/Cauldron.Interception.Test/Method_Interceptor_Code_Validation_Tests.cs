using Cauldron.Core.Extensions;
using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Method_Interceptor_Code_Validation_Tests
    {
        [TestMethod]
        [TestMethodInterceptorWithParameter(
                1, 2, true, 3, 4, 'd', 5, 6, 7, 8, 9.9, 8.8f, "Hi here", 66, "stringMe", typeof(Guid), TestEnum.One,
                new int[] { 3, 4 },
                new uint[] { 5, 6 },
                new bool[] { true, false, true },
                new byte[] { 4, 6, 33 },
                new sbyte[] { 43, 8 },
                new char[] { 'ö', 'ä' },
                new short[] { 43, 2 },
                new ushort[] { 3, 6 },
                new long[] { 12, 3 },
                new ulong[] { 45, 3 },
                new double[] { 4.5, 6.2 },
                new float[] { 34f, 6.7f },
                new string[] { "Hello", "My Friend" },
                null,
                null,
                null,
                null)]

        //[TestMethodInterceptorWithParameter(
        //        1, 2, true, 3, 4, 'd', 5, 6, 7, 8, 9.9, 8.8f, "Hi here", 66, "stringMe", typeof(Guid), TestEnum.One,
        //        new int[] { 3, 4 },
        //        new uint[] { 5, 6 },
        //        new bool[] { true, false, true },
        //        new byte[] { 4, 6, 33 },
        //        new sbyte[] { 43, 8 },
        //        new char[] { 'ö', 'ä' },
        //        new short[] { 43, 2 },
        //        new ushort[] { 3, 6 },
        //        new long[] { 12, 3 },
        //        new ulong[] { 45, 3 },
        //        new double[] { 4.5, 6.2 },
        //        new float[] { 34f, 6.7f },
        //        new string[] { "Hello", "My Friend" },
        //        new object[] { 88, 23, "kool" },
        //        new object[] { "mmm", 45f, 3.4, false },
        //        new Type[] { typeof(Guid), typeof(string), typeof(int) },
        //        new TestEnum[] { TestEnum.One, TestEnum.Three })]
        public void Attribute_Parameter()
        {
            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Class_Method_With_Multiple_Returns_Object()
        {
            Assert.AreEqual(200, this.Class_Method_With_Multiple_Returns_Object_(0));
            Assert.AreEqual("hello", this.Class_Method_With_Multiple_Returns_Object_(1));
            Assert.AreEqual(99.99, this.Class_Method_With_Multiple_Returns_Object_(5));
        }

        [TestMethod]
        public void Class_Method_With_Single_Return()
        {
            var result = this.Class_Method_With_Single_Return_();
            Assert.AreEqual(33, result.IntegerProperty);
            Assert.AreEqual("Hello", result.StringProperty);
        }

        [TestMethod]
        public void Class_Method_With_Single_Return_Interface()
        {
            var result = this.Class_Method_With_Single_Return_Interface_();
            Assert.AreEqual(33, result.IntegerProperty);
            Assert.AreEqual("Hello", result.StringProperty);
        }

        [TestMethod]
        public void ValueType_Method_With_Multiple_Returns()
        {
            Assert.AreEqual(200, this.ValueType_Method_With_Multiple_Returns_(0));
            Assert.AreEqual(-812209447, this.ValueType_Method_With_Multiple_Returns_(1));
            Assert.AreEqual(99, this.ValueType_Method_With_Multiple_Returns_(5));
            Assert.AreEqual(2429, this.ValueType_Method_With_Multiple_Returns_(2));
            Assert.AreEqual(45, this.ValueType_Method_With_Multiple_Returns_(1154));
        }

        [TestMethod]
        public void ValueType_Method_With_Multiple_Returns_And_TryCatch()
        {
            Assert.AreEqual(200, this.ValueType_Method_With_Multiple_Returns_And_TryCatch_(0));
            Assert.AreEqual(-812209447, this.ValueType_Method_With_Multiple_Returns_And_TryCatch_(1));
            Assert.AreEqual(99, this.ValueType_Method_With_Multiple_Returns_And_TryCatch_(5));
            Assert.AreEqual(2429, this.ValueType_Method_With_Multiple_Returns_And_TryCatch_(2));
            Assert.AreEqual(45, this.ValueType_Method_With_Multiple_Returns_And_TryCatch_(1154));
        }

        [TestMethod]
        public void ValueType_Method_With_Multiple_Returns_Async()
        {
            Assert.AreEqual(200, this.ValueType_Method_With_Multiple_Returns_Async_(0).RunSync());
            Assert.AreEqual(-812209447, this.ValueType_Method_With_Multiple_Returns_Async_(1).RunSync());
            Assert.AreEqual(99, this.ValueType_Method_With_Multiple_Returns_Async_(5).RunSync());
            Assert.AreEqual(2429, this.ValueType_Method_With_Multiple_Returns_Async_(2).RunSync());
            Assert.AreEqual(45, this.ValueType_Method_With_Multiple_Returns_Async_(1154).RunSync());
        }

        [TestMethod]
        public void ValueType_Method_With_Single_Return()
        {
            var result = this.ValueType_Method_With_Single_Return_();
            Assert.AreEqual(3434, result);
        }

        [TestMethod]
        public void ValueType_Method_With_Single_Return_Object()
        {
            var result = this.ValueType_Method_With_Single_Return_Object_();
            Assert.AreEqual(3434, result);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns()
        {
            var username = Environment.UserName;

            if (username == "batman")
                return;

            if (username == "catwoman")
            {
                var t = Environment.Version.ToString() + Environment.SystemDirectory;
                username = t;
                return;
            }

            if (username == "bla")
            {
                if (username != "300")
                {
                    username = 7788.ToString();
                    return;
                }

                Console.WriteLine("Nothing");
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns_And_Action()
        {
            var action = new Action(() =>
            {
                var username = Environment.UserName;

                if (username == "batman")
                    return;

                if (username == "catwoman")
                {
                    var t = Environment.Version.ToString() + Environment.SystemDirectory;
                    username = t;
                    return;
                }

                if (username == "bla")
                {
                    if (username != "300")
                    {
                        username = 7788.ToString();
                        return;
                    }

                    Console.WriteLine("Nothing");
                }
            });

            action();

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns_And_TryCatch()
        {
            try
            {
                var username = Environment.UserName;

                if (username == "batman")
                    return;

                if (username == "catwoman")
                {
                    var t = Environment.Version.ToString() + Environment.SystemDirectory;
                    username = t;
                    return;
                }

                if (username == "bla")
                {
                    if (username != "300")
                    {
                        username = 7788.ToString();
                        return;
                    }

                    Console.WriteLine("Nothing");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Nothing");
                throw;
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Single_Return()
        {
            var i = 3 + 5;
            var date = DateTime.Now.AddMonths(i);

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Single_Return_And_Function()
        {
            var func = new Func<DateTime>(() =>
            {
                var i = 3 + 5;
                var date = DateTime.Now.AddMonths(i);
                return date;
            });

            var tt = func();

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Single_Return_And_Try_Catch()
        {
            try
            {
                var i = 3 + 5;
                var date = DateTime.Now.AddMonths(i);
            }
            catch (Exception)
            {
                Console.WriteLine("Nothing");
                throw;
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        private object Class_Method_With_Multiple_Returns_Object_(int index)
        {
            if (index == 0)
                return 200;

            if (index == 1)
            {
                return "hello";
            }

            if (index == 2 || index == 5)
            {
                if (index * 2 == ((int)index / 2) * 4)
                {
                    var zu = Guid.NewGuid();
                    return zu;
                }

                return 99.99;
            }

            return new TestClass { EnumProperty = TestEnum.Three };
        }

        [TestMethodInterceptor]
        private TestClass Class_Method_With_Single_Return_()
        {
            return new TestClass { CharProperty = 'e', IntegerProperty = 33, StringProperty = "Hello" };
        }

        [TestMethodInterceptor]
        private ITestInterface Class_Method_With_Single_Return_Interface_()
        {
            return new TestClass { CharProperty = 'e', IntegerProperty = 33, StringProperty = "Hello" };
        }

        private void HUHUU()
        {
            var tt = new TestMethodInterceptorWithParameter(
                1, 2, true, 3, 4, 'd', 5, 6, 7, 8, 9.9, 8.8f, "Hi here", 66, "stringMe", typeof(Guid), TestEnum.One,
                new int[] { 3, 4 },
                new uint[] { 5, 6 },
                new bool[] { true, false, true },
                new byte[] { 4, 6, 33 },
                new sbyte[] { 43, 8 },
                new char[] { 'ö', 'ä' },
                new short[] { 43, 2 },
                new ushort[] { 3, 6 },
                new long[] { 12, 3 },
                new ulong[] { 45, 3 },
                new double[] { 4.5, 6.2 },
                new float[] { 34f, 6.7f },
                new string[] { "Hello", "My Friend" },
                new object[] { 88, 23, "kool" },
                new object[] { "mmm", 45f, 3.4, false },
                new Type[] { typeof(Guid), typeof(string), typeof(int) },
                new TestEnum[] { TestEnum.One, TestEnum.Three });
        }

        [TestMethodInterceptor]
        private int ValueType_Method_With_Multiple_Returns_(int index)
        {
            if (index == 0)
                return 200;

            if (index == 1)
            {
                var test = 3482757849;
                return test.GetHashCode();
            }

            if (index == 2 || index == 5)
            {
                if (index * 2 == ((int)index / 2) * 4)
                {
                    var zu = 643 + 8934 / 5;
                    return zu;
                }

                return 99;
            }

            return 45;
        }

        [TestMethodInterceptor]
        private int ValueType_Method_With_Multiple_Returns_And_TryCatch_(int index)
        {
            try
            {
                if (index == 0)
                    return 200;

                if (index == 1)
                {
                    var test = 3482757849;
                    return test.GetHashCode();
                }

                if (index == 2 || index == 5)
                {
                    if (index * 2 == ((int)index / 2) * 4)
                    {
                        var zu = 643 + 8934 / 5;
                        return zu;
                    }

                    return 99;
                }

                return 45;
            }
            catch (Exception)
            {
                Console.WriteLine("Nothing");
                throw;
            }
        }

        [TestMethodInterceptor]
        private async Task<int> ValueType_Method_With_Multiple_Returns_Async_(int index)
        {
            return await Task.Run(() =>
           {
               if (index == 0)
                   return 200;

               if (index == 1)
               {
                   var test = 3482757849;
                   return test.GetHashCode();
               }

               if (index == 2 || index == 5)
               {
                   if (index * 2 == ((int)index / 2) * 4)
                   {
                       var zu = 643 + 8934 / 5;
                       return zu;
                   }

                   return 99;
               }

               return 45;
           });
        }

        [TestMethodInterceptor]
        private int ValueType_Method_With_Single_Return_()
        {
            return 3434;
        }

        [TestMethodInterceptor]
        private object ValueType_Method_With_Single_Return_Object_()
        {
            return 3434;
        }
    }
}