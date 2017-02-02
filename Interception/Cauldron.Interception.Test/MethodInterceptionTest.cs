using Cauldron.Core.Interceptors;
using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class MethodInterceptionTest
    {
        private readonly IMethodInterceptor _TestMethod1_interceptor__ = new TestMethodInterceptorAttribute();

        [TestMethodInterceptor]
        public void TestImplemetation()
        {
            Console.WriteLine("Hello");
        }

        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                this._TestMethod1_interceptor__.OnEnter();
            }
            catch (Exception e)
            {
                this._TestMethod1_interceptor__.OnException(e);
            }
            finally
            {
                this._TestMethod1_interceptor__.OnExit();
            }
        }
    }
}