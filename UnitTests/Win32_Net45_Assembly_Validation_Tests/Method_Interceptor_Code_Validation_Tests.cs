using Microsoft.VisualStudio.TestTools.UnitTesting;
using Win32_Net45_Assembly_Validation_Tests.Interceptors;

namespace Win32_Net45_Assembly_Validation_Tests
{
    [TestClass]
    public class Method_Interceptor_Code_Validation_Tests
    {
        [TestMethodInterceptor]
        public void Simple_Method()
        {
        }
    }
}