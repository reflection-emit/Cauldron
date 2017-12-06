using Cauldron.Cryptography;
using System.Linq;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Test
{
    /// <summary>
    /// These tests are meant for compitibility tests with the desktop version
    /// </summary>
    [TestClass]
    public class KeyMaterial_Test
    {
        [TestMethod]
        public void Create_Key_Material_And_Test_Key()
        {
            var salt = new byte[] { 14, 11, 202, 234, 78, 19, 148, 146, 17, 71, 187, 141, 123, 7, 124, 192 };

            var key = new byte[] { 181, 48, 37, 36, 6, 21, 63, 251, 220, 225, 106, 33, 134, 235, 58, 217, 206, 17, 10, 63, 253, 59, 205, 162, 134, 47, 228, 218, 63, 70, 115, 0 };
            var initializationVector = new byte[] { 181, 48, 37, 36, 6, 21, 63, 251, 220, 225, 106, 33, 134, 235, 58, 217 };

            using (var keyMaterial = KeyMaterial.CreateKeyMaterial("password1".ToSecureString(), salt, 2000))
            {
                Assert.IsTrue(key.SequenceEqual(keyMaterial.Key));
                Assert.IsTrue(initializationVector.SequenceEqual(keyMaterial.InitializationVector));
            }
        }
    }
}