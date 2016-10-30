using Cauldron.Core.Extensions;
using System;
using System.Security;
using System.Text;

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Encrypts the data with AES and a random key.
    /// The Key itself will be encrypted with RSA
    /// </summary>
    public static class RsaAes
    {
        /// <summary>
        /// Decrypts encrypted data
        /// </summary>
        /// <param name="privateKey">The private key used to decrypt</param>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The decrypted data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="privateKey"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        [SecurityCritical]
        public static byte[] Decrypt(SecureString privateKey, byte[] data)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var rsaOnly = data[data.Length - 1] == 0;

            if (rsaOnly)
            {
                byte[] d = new byte[data.Length - 1];

                Array.Copy(data, 0, d, 0, d.Length);
                return Rsa.Decrypt(privateKey, d);
            }
            else
            {
                byte[] keyLength = new byte[4];

                Array.Copy(data, data.Length - 5, keyLength, 0, 4);

                byte[] k = new byte[keyLength.ToInteger()];
                byte[] d = new byte[data.Length - k.Length - 5];

                Array.Copy(data, k, k.Length);
                Array.Copy(data, k.Length, d, 0, d.Length);

                var key = Rsa.Decrypt(privateKey, k);
                byte[] decryptedData = Aes.Decrypt(key.ToSecureString(), d);

                return decryptedData;
            }
        }

        /// <summary>
        /// Encrypts data
        /// </summary>
        /// <param name="publicKey">The public key to use to encrypt the data</param>
        /// <param name="data">The data to encrypt. The string data will be converted to bytes using <see cref="Encoding.UTF8"/></param>
        /// <returns>The ecrypted data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="publicKey"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="data"/> is empty</exception>
        public static byte[] Encrypt(string publicKey, string data)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data cannot be empty");

            return Encrypt(publicKey, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Encrypts data
        /// </summary>
        /// <param name="publicKey">The public key to use to encrypt the data</param>
        /// <param name="data">The data to encrypt</param>
        /// <returns>The ecrypted data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="publicKey"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Encrypt(string publicKey, byte[] data)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] result = null;

            // If the RSA key size allows us to encrypt the data without AES then... Let us do that
            if (Convert.FromBase64String(publicKey).Length - 10 >= data.Length)
            {
                var d = Rsa.Encrypt(publicKey, data);

                result = new byte[d.Length + 1];
                Array.Copy(d, 0, result, 0, d.Length);

                // The zero in the end of the encrypted data indicates us that
                // there is no AES in place
                result[result.Length - 1] = 0;
            }
            else
            {
                using (var randomPassword = (CryptoUtils.BrewPassword(80) + publicKey + DateTime.Now.ToString()).GetHash(Core.HashAlgorithms.Sha512).ToSecureString())
                {
                    var d = Aes.Encrypt(randomPassword, data);
                    var k = Rsa.Encrypt(publicKey, randomPassword.GetBytes());
                    var keyLength = k.Length.ToBytes();

                    result = new byte[k.Length + d.Length + 4 + 1];

                    Array.Copy(k, result, k.Length);
                    Array.Copy(d, 0, result, k.Length, d.Length);
                    Array.Copy(keyLength, 0, result, k.Length + d.Length, 4);

                    result[result.Length - 1] = 1;
                }
            }

            return result;
        }
    }
}