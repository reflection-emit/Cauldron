using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Couldron.Core.Cryptography
{
    /// <summary>
    /// Provides methods to encrypt and decrypt data with AES
    /// </summary>
    public static class AES
    {
        #region Encryption

        public static byte[] Encrypt(SecureString password, string data)
        {
            return Encrypt(password, Encoding.Unicode.GetBytes(data));
        }

        public static byte[] Encrypt(SecureString password, byte[] data)
        {
            return Encrypt(KeyMaterial.CreateKeyMaterial(password), data);
        }

        public static byte[] Encrypt(SecureString password, SecureString data)
        {
            using (var handler = new SecureStringHandler(data))
            {
                return Encrypt(KeyMaterial.CreateKeyMaterial(password), handler.ToBytes());
            }
        }

        private static byte[] Encrypt(KeyMaterial keyMaterial, byte[] data)
        {
            byte[] result;

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = keyMaterial.InitialisationVector;
                aes.Key = keyMaterial.Key;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                    }

                    result = ms.ToArray();
                }
            }

            Array.Resize(ref result, result.Length + 16);
            Array.Copy(keyMaterial.Salt, 0, result, result.Length - 16, 16);

            return Compression.Compress(result);
        }

        #endregion Encryption

        public static byte[] Decrypt(SecureString password, byte[] data)
        {
            byte[] uncompressedData = Compression.Decompress(data);
            byte[] salt = new byte[16];
            byte[] encrypted = new byte[uncompressedData.Length - 16];

            Array.Copy(uncompressedData, uncompressedData.Length - 16, salt, 0, 16);
            Array.Copy(uncompressedData, 0, encrypted, 0, encrypted.Length);

            var keyMaterial = KeyMaterial.CreateKeyMaterial(password, salt);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = keyMaterial.InitialisationVector;
                aes.Key = keyMaterial.Key;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encrypted, 0, encrypted.Length);
                        cs.Close();
                    }

                    return ms.ToArray();
                }
            }
        }

        [SecurityCritical]
        public static SecureString DecryptToSecureString(SecureString password, byte[] data)
        {
            var decryptedData = Decrypt(password, data);
            var decryptedDataGC = GCHandle.Alloc(decryptedData, GCHandleType.Pinned);
            var charArray = Encoding.ASCII.GetChars(Compression.Decompress(decryptedData));
            var charArrayGC = GCHandle.Alloc(charArray, GCHandleType.Pinned);

            decryptedDataGC.RandomizeValues(decryptedData.Length);

            var result = new SecureString();

            for (int i = 0; i < charArray.Length; i++)
                result.AppendChar(charArray[i]);

            charArrayGC.RandomizeValues(charArray.Length);

            return result;
        }
    }
}