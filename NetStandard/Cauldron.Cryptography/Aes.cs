using Cauldron.Core.Extensions;
using System;
using System.IO;
using System.Security;
using System.Text;

#if WINDOWS_UWP

using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Provides methods to encrypt and decrypt data with AES
    /// </summary>
    public static class Aes
    {
        #region Encryption

        /// <summary>
        /// Encrypts a string data
        /// </summary>
        /// <param name="password">The password to use to encrypt the data</param>
        /// <param name="data">
        /// The data to encrypt. The string data will be converted to bytes using <see cref="Encoding.UTF8"/>
        /// </param>
        /// <returns>The encrypted data represented by bytes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Encrypt(SecureString password, string data) =>
            Encrypt(password, Encoding.UTF8.GetBytes(data));

        /// <summary>
        /// Encrypts a binary data
        /// </summary>
        /// <param name="password">The password to use to encrypt the data</param>
        /// <param name="data">The data to encrypt</param>
        /// <returns>The encrypted data represented by bytes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Encrypt(SecureString password, byte[] data)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var keyMaterial = KeyMaterial.CreateKeyMaterial(password))
                return Encrypt(keyMaterial, data);
        }

        /// <summary>
        /// Encrypts a string data
        /// </summary>
        /// <param name="keyMaterial">The <see cref="KeyMaterial"/> to use to encrypt the data</param>
        /// <param name="data">The data to encrypt</param>
        /// <returns>The encrypted data represented by bytes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyMaterial"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Encrypt(KeyMaterial keyMaterial, byte[] data)
        {
            if (keyMaterial == null)
                throw new ArgumentNullException(nameof(keyMaterial));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] result;

#if WINDOWS_UWP

            var toDecryptBuffer = CryptographicBuffer.CreateFromByteArray(data);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyMaterial.Key.AsBuffer());

            // The input key must be securely shared between the sender of the cryptic message and
            // the recipient. The initialization vector must also be shared but does not need to be
            // shared in a secure manner. If the sender encodes a message string to a buffer, the
            // binary encoding method must also be shared with the recipient.
            var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, data.AsBuffer(), keyMaterial.InitializationVector.AsBuffer());

            CryptographicBuffer.CopyToByteArray(buffEncrypted, out result);

#else

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = keyMaterial.InitializationVector;
                aes.Key = keyMaterial.Key;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);

#if NETSTANDARD2_0
                        cs.Close();
#endif
                    }

                    result = ms.ToArray();
                }
            }
#endif

            // TODO - has to be optimized... Get rid of resizing
            Array.Resize(ref result, result.Length + 16);
            Array.Copy(keyMaterial.Salt, 0, result, result.Length - 16, 16);

            return result.ZipAsBytes();
        }

        #endregion Encryption

        /// <summary>
        /// Descrypts encrypted data
        /// </summary>
        /// <param name="password">The password used to decrypt data</param>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The decrypted data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Decrypt(SecureString password, byte[] data) =>
            Aes.Decrypt(password, KeyMaterial.DefaultIterations, data);

        /// <summary>
        /// Descrypts encrypted data
        /// </summary>
        /// <param name="password">The password used to decrypt data</param>
        /// <param name="iterations">The number of iteration for the KDF</param>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The decrypted data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        /// <exception cref="SecurityException">
        /// <paramref name="iterations"/> is lower than 1000
        /// </exception>
        public static byte[] Decrypt(SecureString password, int iterations, byte[] data)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (iterations < 1000)
                throw new SecurityException("The KDF iterations should not be lower than 1000");

            var uncompressedData = data.UnzipAsBytes();
            var salt = new byte[16];
            var encrypted = new byte[uncompressedData.Length - 16];

            Array.Copy(uncompressedData, uncompressedData.Length - 16, salt, 0, 16);
            Array.Copy(uncompressedData, 0, encrypted, 0, encrypted.Length);

            using (var keyMaterial = KeyMaterial.CreateKeyMaterial(password, salt, iterations))
            {
#if WINDOWS_UWP

                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

                var symetricKey = aes.CreateSymmetricKey(keyMaterial.Key.AsBuffer());
                var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, encrypted.AsBuffer(), keyMaterial.InitializationVector.AsBuffer());

                byte[] result;

                CryptographicBuffer.CopyToByteArray(buffDecrypted, out result);

                return result;

#else
                using (var aes = System.Security.Cryptography.Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = keyMaterial.InitializationVector;
                    aes.Key = keyMaterial.Key;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encrypted, 0, encrypted.Length);

#if NETSTANDARD2_0
                            cs.Close();
#endif
                        }

                        return ms.ToArray();
                    }
                }
#endif
            }
        }
    }
}