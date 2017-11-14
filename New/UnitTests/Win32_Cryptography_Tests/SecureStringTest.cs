using Cauldron.Cryptography;
using Cauldron;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Win32_Cryptography_Tests
{
    [TestClass]
    public class SecureStringTest
    {
        [TestMethod]
        public void Create_SecureString_And_Get_Value()
        {
            string password = "This is a test password 565";
            using (var secureString = password.ToSecureString())
            {
                Assert.AreEqual(password, secureString.GetString());
            }
        }
    }
}