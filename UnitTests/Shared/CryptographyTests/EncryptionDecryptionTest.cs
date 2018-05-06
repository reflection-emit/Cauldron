using System.Text;
using Cauldron;
using Cauldron.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptographyTests
{
    [TestClass]
    public class EncryptionDecryptionTest
    {
        [TestMethod]
        public void AES_Encrypt_Decrypt_Test()
        {
            var password = CryptoUtils.BrewPassword(10).ToSecureString();
            var testData = "Test Test Hello";
            var encrypted = Aes.Encrypt(password, testData);
            var decrypted = Encoding.UTF8.GetString(Aes.Decrypt(password, encrypted));

            Assert.AreEqual(testData, decrypted);
        }

        [TestMethod]
        public void RSA_Encrypt_Decrypt_Test()
        {
            var keyPair = Rsa.CreateKeyPair(RSAKeySizes.Size1024);
            var testData = "Test Test Hello";
            var encrypted = Rsa.Encrypt(keyPair.PublicKey, testData);
            var decrypted = Encoding.UTF8.GetString(Rsa.Decrypt(keyPair.PrivateKey, encrypted));

            Assert.AreEqual(testData, decrypted);
        }
    }
}