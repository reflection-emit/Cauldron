using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UnitTest_InterceptorsForTest;

namespace UnitTest_BasicInterceptorTests
{
    [TestClass]
    public class SimpleMethod_Interceptor_Code_Validation_Tests
    {
        [SimpleInterceptor]
        public string AMethod()
        {
            return ",,";
        }

        [SimpleInterceptor]
        public string AMethod(string bo, int zu, Guid io)
        {
            return ",,";
        }

        [SimpleInterceptorWithoutInstance]
        public string AMethodWithSingleInstance()
        {
            return "huhu";
        }

        [SimpleInterceptorWithSyncAndInstance]
        public double AMethodWithSingleInstanceWithSyncRoot()
        {
            return 4.4;
        }

        [SimpleInterceptor]
        public async Task<string> Async_AMethod()
        {
            return await Task.Run(() => ",,");
        }

        [SimpleInterceptor]
        public async Task<string> Async_AMethod(string bo, int zu, Guid io)
        {
            return await Task.Run(() => ",,");
        }

        [SimpleInterceptorWithoutInstance]
        public async Task<string> Async_AMethodWithSingleInstance()
        {
            return await Task.Run(() => "huhu");
        }

        [SimpleInterceptorWithSyncAndInstance]
        public async Task<double> Async_AMethodWithSingleInstanceWithSyncRoot()
        {
            return await Task.Run(() => 4.4);
        }

        [SimpleInterceptor]
        [SimpleInterceptorWithSyncAndInstance]
        [SimpleInterceptorWithoutInstance]
        [SimpleInterceptorWithSync]
        public async Task<double> Async_MultiInterceptor()
        {
            return await Task.Run(() => 44.00);
        }

        [SimpleInterceptorWithSync]
        public async Task<int> Async_WithSyncRoot()
        {
            return await Task.Run(() => 44);
        }

        [SimpleInterceptor]
        [SimpleInterceptorWithSyncAndInstance]
        [SimpleInterceptorWithoutInstance]
        [SimpleInterceptorWithSync]
        public double MultiInterceptor()
        {
            return 44.00;
        }

        [InterceptorWithoutInstance]
        [SimpleInterceptor]
        public string SimpleInterceptorWithMethodInterceptorMix(string bo, int zu, Guid io)
        {
            return ",,";
        }

        [SimpleInterceptorWithSync]
        public int WithSyncRoot()
        {
            return 44;
        }
    }
}