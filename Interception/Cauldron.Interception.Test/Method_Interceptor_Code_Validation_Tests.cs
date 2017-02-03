using Cauldron.Core.Interceptors;
using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Method_Interceptor_Code_Validation_Tests
    {
        public string TestMethod()
        {
            return "Loop";
        }

        [TestMethod]
        public void ValueType_Method_With_Multiple_Returns_First()
        {
            Assert.AreEqual(200, this.ValueType_Method_With_Multiple_Returns(0));
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
        public void Void_Method_With_Single_Return()
        {
            var i = 3 + 5;
            var date = DateTime.Now.AddMonths(i);

            // If the unit test was successfully invoked, then we are sure that our weaver weaved the whole thing correctly
            Assert.AreEqual(true, true);
        }

        [TestMethodInterceptor]
        private int ValueType_Method_With_Multiple_Returns(int index)
        {
            if (index == 0)
                return 200;

            if (index == 1)
            {
                var test = 3482757849;
                return test.GetHashCode();
            }

            if (index == 2)
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

        private int ValueType_Method_With_Multiple_Returns_Sample(int index)
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

                if (index == 2)
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
                throw;
            }
            finally
            {
            }
        }
    }
}