using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

#if WINDOWS_UWP

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Provides methods to create a key material
    /// </summary>
    public sealed class KeyMaterial : DisposableBase
    {
        /// <summary>
        /// The default KDF iterations
        /// </summary>
        public const int DefaultIterations = 60000;

        private byte[] initialisationVector;
        private GCHandle initialisationVectorHandle;
        private byte[] key;
        private GCHandle keyHandle;
        private byte[] salt;
        private GCHandle saltHandle;

        private KeyMaterial(SecureString password, byte[] salt, int iterations)
        {
            if (iterations < 1000)
                throw new SecurityException("The KDF iterations should not be lower than 1000");

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (salt == null)
                throw new ArgumentNullException(nameof(salt));

            // Possibly security critical
#if WINDOWS_UWP
            var passwordBuffer = CryptographicBuffer.ConvertStringToBinary(password.GetString(), BinaryStringEncoding.Utf8);
            var saltBuffer = CryptographicBuffer.CreateFromByteArray(salt);

            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, (uint)iterations);

            var keyOriginal = keyDerivationProvider.CreateKey(passwordBuffer);
            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            var derivedPasswordKey = keyDerivationProvider.CreateKey(passwordBuffer);
            this.key = keyMaterial.ToArray().Compress();
            this.initialisationVector = keyMaterial.ToArray().GetBytes(16).Compress();

#else
            using (Rfc2898DeriveBytes keyMaterial = new Rfc2898DeriveBytes(password.GetString(), salt, iterations))
            {
                this.key = keyMaterial.GetBytes(32).Compress();
                keyMaterial.Reset();
                this.initialisationVector = keyMaterial.GetBytes(16).Compress();
            }
#endif

            this.salt = salt.Compress();

            // Let us pin our variables so that the GC is not moving them around creating a lot of copies
            this.saltHandle = GCHandle.Alloc(this.salt, GCHandleType.Pinned);
            this.initialisationVectorHandle = GCHandle.Alloc(this.initialisationVector, GCHandleType.Pinned);
            this.keyHandle = GCHandle.Alloc(this.key, GCHandleType.Pinned);
        }

        /// <summary>
        /// Gets the initialization vector for the symmethric algorithm
        /// </summary>
        public byte[] InitializationVector
        {
            get { return this.initialisationVector.Uncompress(); }
        }

        /// <summary>
        /// Gets the secret key for the symmethric algorithm
        /// </summary>
        public byte[] Key
        {
            get { return this.key.Uncompress(); }
        }

        /// <summary>
        /// Gets the salt for the symmethric algorithm
        /// </summary>
        public byte[] Salt
        {
            get { return this.salt.Uncompress(); }
        }

        /// <summary>
        /// Creates a key material for the symmethric algorithm
        /// </summary>
        /// <param name="password">The password used for the encryption or description</param>
        /// <param name="salt">The salt for the symmethric algorithm</param>
        /// <param name="iterations">The number of iteration for the KDF</param>
        /// <returns>The key material</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="salt"/> is null</exception>
        /// <exception cref="SecurityException"><paramref name="iterations"/> is lower than 1000</exception>
        public static KeyMaterial CreateKeyMaterial(SecureString password, byte[] salt, int iterations = DefaultIterations) =>
            new KeyMaterial(password, salt, iterations);

        /// <summary>
        /// Creates a key material for the symmethric algorithm. The salt will be generated using the hash of a 42 character long generated string.
        /// </summary>
        /// <param name="password">The password used for the encryption or description</param>
        /// <param name="iterations">The number of iteration for the KDF</param>
        /// <returns>The key material</returns>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="SecurityException"><paramref name="iterations"/> is lower than 1000</exception>
        public static KeyMaterial CreateKeyMaterial(SecureString password, int iterations = DefaultIterations) =>
            new KeyMaterial(password, UTF8Encoding.UTF8.GetBytes(CryptoUtils.BrewPassword(42).GetHash(HashAlgorithms.Sha256)).GetBytes(16), iterations);

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (!disposeManaged)
            {
                this.saltHandle.FillWithRandomValues(this.salt.Length);
                this.initialisationVectorHandle.FillWithRandomValues(this.initialisationVector.Length);
                this.keyHandle.FillWithRandomValues(this.key.Length);
            }
        }
    }
}
