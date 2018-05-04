using System;
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
    /// Provides methods to encrypt and decrypt data with RSA
    /// </summary>
    public static class Rsa
    {
        private const int SALT_LENGTH = 8;

        /// <summary>
        /// Creates a public and private key pair that can be used for asymmethric encryption
        /// </summary>
        /// <param name="keySize">The keysize to generate</param>
        /// <returns>The generated key size</returns>
        public static KeyPair CreateKeyPair(RSAKeySizes keySize)
        {
#if WINDOWS_UWP
            var asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            var key = asym.CreateKeyPair((uint)keySize);

            var privateKeyBuffer = key.Export(CryptographicPrivateKeyBlobType.Capi1PrivateKey);
            var publicKeyBuffer = key.ExportPublicKey(CryptographicPublicKeyBlobType.Capi1PublicKey);

            byte[] privateKeyBytes;
            byte[] publicKeyBytes;

            CryptographicBuffer.CopyToByteArray(privateKeyBuffer, out privateKeyBytes);
            CryptographicBuffer.CopyToByteArray(publicKeyBuffer, out publicKeyBytes);

            string privateKey = Convert.ToBase64String(privateKeyBytes);
            string publicKey = Convert.ToBase64String(publicKeyBytes);

#else
            var cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };
            var rsaProvider = new RSACryptoServiceProvider((int)keySize, cspParams);

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

#endif
            return new KeyPair(privateKey.ToSecureString(), publicKey);
        }

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
#if WINDOWS_UWP
            var keyBuffer = CryptographicBuffer.DecodeFromBase64String(privateKey.GetString());

            var asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            var key = asym.ImportKeyPair(keyBuffer, CryptographicPrivateKeyBlobType.Capi1PrivateKey);

            var plainBuffer = CryptographicEngine.Decrypt(key, data.AsBuffer(), null);

            byte[] plainBytes;
            CryptographicBuffer.CopyToByteArray(plainBuffer, out plainBytes);

            return plainBytes;
#else

            var cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };
            var rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey.GetString()));
            return rsaProvider.Decrypt(data, false);
#endif
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
#if WINDOWS_UWP
            var keyBuffer = CryptographicBuffer.DecodeFromBase64String(publicKey);

            var asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            var key = asym.ImportPublicKey(keyBuffer, CryptographicPublicKeyBlobType.Capi1PublicKey);

            var plainBuffer = CryptographicBuffer.CreateFromByteArray(data);
            var encryptedBuffer = CryptographicEngine.Encrypt(key, plainBuffer, null);

            byte[] encryptedBytes;
            CryptographicBuffer.CopyToByteArray(encryptedBuffer, out encryptedBytes);

            return encryptedBytes;
#else

            var cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };
            var rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

            return rsaProvider.Encrypt(data, false);
#endif
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

            return Rsa.Encrypt(publicKey, Encoding.UTF8.GetBytes(data));
        }
    }
}