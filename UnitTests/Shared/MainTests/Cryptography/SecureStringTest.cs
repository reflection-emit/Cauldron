using Cauldron;
using Cauldron.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Cryptography
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