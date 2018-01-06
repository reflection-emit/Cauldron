using Cauldron;
using Cauldron.Core;
using Cauldron.Core.Diagnostics;
using Fody_Assembly_Validation_External_Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [TestClass]
    public class Method_Interceptor_Code_Validation_Tests
    {
        [ExternalLockableMethod]
        public static void Static_Lockable_Method_()
        {
            var uu = Randomizer.GenerateLoremIpsum(4, 8);
            System.Diagnostics.Debug.WriteLine(uu);
        }

        [TestMethod]
        public void Async_Method_Interception_Try_Catch_Correctly_Catching()
        {
            bool exception = false;

            try
            {
                Method_1_Async().RunSync();
            }
            catch (Exception e)
            {
                exception = e.GetBaseException().GetType() == typeof(DivideByZeroException);
            }
            Assert.AreEqual(true, exception);
        }

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
                new object[] { 88, 23, "kool" },
                new object[] { "mmm", 45f, 3.4, false },
                new Type[] { typeof(Guid), typeof(string), typeof(int) },
                new TestEnum[] { TestEnum.One, TestEnum.Three })]
        public void Attribute_Parameter()
        {
            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
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

        [ExternalLockableMethod]
        [TestMethod]
        public void Lockable_Method()
        {
            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly

            var uu = Randomizer.GenerateLoremIpsum(4, 6);
            System.Diagnostics.Debug.WriteLine(uu);
        }

        [TestMethodInterceptor]
        [TestMethod]
        [ExternalLockableMethod]
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
               new object[] { 88, 23, "kool" },
               new object[] { "mmm", 45f, 3.4, false },
               new Type[] { typeof(Guid), typeof(string), typeof(int) },
               new TestEnum[] { TestEnum.One, TestEnum.Three })]
        [ExternalMethodInterceptor("test_opop", 2342)]
        public void Multiple_Attributes()
        {
            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Static_Lockable_Method()
        {
            Static_Lockable_Method_();
        }

        [TestMethod]
        public void Static_ValueType_Method_With_Single_Return()
        {
            var result = Static_ValueType_Method_With_Single_Return_();
            Assert.AreEqual(3434, result);
        }

        [TestMethod, TestMethodInterceptor]
        public void SwitchTest()
        {
            var test = "asdf";
            // TODO - Test does not preform correct tests
            switch (UserInformation.UserName.GetHashCode())
            {
                case 55:
                case 1:
                    test = "ioi";
                    break;

                case 0:
                case 77:
                    test = "";

                    break;

                case 834:
                    return;

                default:
                    test = "io645764i";

                    break;
            }

            Assert.IsTrue(!string.IsNullOrEmpty(test));
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
            var username = UserInformation.UserName;

            if (username == "batman")
                return;

            if (username == "catwoman")
            {
                var t = Debug.HardwareIdentifier;
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

                var uu = Randomizer.GenerateLoremIpsum(4, 7);
                System.Diagnostics.Debug.WriteLine(uu);
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns_And_Action()
        {
            var action = new Action(() =>
            {
                var username = UserInformation.UserName;

                if (username == "batman")
                    return;

                if (username == "catwoman")
                {
                    var t = Debug.HardwareIdentifier;
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

                    var uu = Randomizer.GenerateLoremIpsum(4, 7);
                    System.Diagnostics.Debug.WriteLine(uu);
                }
            });

            action();

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [ExternalMethodInterceptor("hello", 4003)]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns_And_Action_External()
        {
            var action = new Action(() =>
            {
                var username = UserInformation.UserName;

                if (username == "batman")
                    return;

                if (username == "catwoman")
                {
                    var t = Debug.HardwareIdentifier;
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

                    var uu = Randomizer.GenerateLoremIpsum(4, 7);
                    System.Diagnostics.Debug.WriteLine(uu);
                }
            });

            action();

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Multiple_Returns_And_TryCatch()
        {
            try
            {
                var username = UserInformation.UserName;

                if (username == "batman")
                    return;

                if (username == "catwoman")
                {
                    var t = Debug.HardwareIdentifier;
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

                    var uu = Randomizer.GenerateLoremIpsum(4, 6);
                    System.Diagnostics.Debug.WriteLine(uu);
                }
            }
            catch (Exception)
            {
                var uu = Randomizer.GenerateLoremIpsum(4, 6);
                System.Diagnostics.Debug.WriteLine(uu);
                throw;
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Single_Return()
        {
            var i = 3 + 5;
            var date = DateTime.Now.AddMonths(i);

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        [TestMethod]
        public void Void_Method_With_Single_Return_And_Function()
        {
            DateTime func()
            {
                var i = 3 + 5;
                var date = DateTime.Now.AddMonths(i);
                return date;
            }

            var tt = func();

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
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
                var uu = Randomizer.GenerateLoremIpsum(4, 3);
                System.Diagnostics.Debug.WriteLine(uu);
                throw;
            }

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the
            // whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        private static int Static_ValueType_Method_With_Single_Return_()
        {
            return 3434;
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

        private async Task<bool> LongRunningTaskAsync()
        {
            await Task.Run(() =>
            {
                Task.Delay(5000).RunSync();
            });
            return true;
        }

        [TestMethodInterceptor]
        private async Task<bool> Method_1_Async()
        {
            var result = await LongRunningTaskAsync();

            if (result)
                throw new DivideByZeroException();

            return result;
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
                var uu = Randomizer.GenerateLoremIpsum(4, 3);
                System.Diagnostics.Debug.WriteLine(uu);
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