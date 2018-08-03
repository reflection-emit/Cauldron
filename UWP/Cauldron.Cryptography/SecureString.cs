using Cauldron;
using Cauldron.Core.Diagnostics;
using Cauldron.Cryptography;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace System.Security /* So that precompiler definitions are not required if classes are shared between UWP and Desktop */
{
    /// <summary>
    /// Represents text that should be kept confidential. The text is encrypted for privacy when
    /// being used, and deleted from computer memory when no longer needed. This class cannot be inherited.
    /// </summary>
    public sealed class SecureString : DisposableObject
    {
        /*
         *
         * In UWP if we want to pass the certification, we have to be very careful in using pinvoke.
         * In our case the win32 apis that are used to implement the desktop secure string class
         * will fail on certification.
         * We will be trying to implement something that protect our passwords
         * from being readable if it happens to surface in logs or in memory dumps.
         * This implementation is not meant to be completely secure.
         *
         * We will also not be pinning the data, because pinned data makes the GC cry... And we don't want the GC to cry.
         *
         */

        private byte[] data;
        private byte[] initialisationVector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureString"/> class from a subarray of
        /// <see cref="char"/> objects.
        /// </summary>
        /// <param name="value">A pointer to an array of System.Char objects.</param>
        /// <param name="length">The number of elements of value to include in the new instance.</param>
        public unsafe SecureString(char* value, int length)
        {
            this.Length = length;
            this.initialisationVector = Encoding.UTF8.GetBytes(CryptoUtils.BrewPassword(42)).GetBytes(16);

            byte[] dataCopy = new byte[length];
            var gc = GCHandle.Alloc(dataCopy, GCHandleType.Pinned);

            for (int i = 0; i < dataCopy.Length; i++)
                dataCopy[i] = (byte)*(value + i);

            // We cannot use our Aes implemtation in here because we will cause a cycling
            // dependency... And that will lead to a StackOverflow

            var passPhrase = Encoding.UTF8.GetBytes(Debug.HardwareIdentifier.GetHash(HashAlgorithms.Sha512));
            var keyMaterial = CryptographicBuffer.CreateFromByteArray(passPhrase);

            var toDecryptBuffer = CryptographicBuffer.CreateFromByteArray(dataCopy);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var symetricKey = aes.CreateSymmetricKey(keyMaterial);
            var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, dataCopy.AsBuffer(), initialisationVector.AsBuffer());

            CryptographicBuffer.CopyToByteArray(buffEncrypted, out data);

            gc.FillWithRandomValues(dataCopy.Length);
        }

        /// <summary>
        /// Gets the number of characters in the current secure string.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Returns the <see cref="SecureString"/> value as an array of bytes
        /// </summary>
        /// <returns>An array of bytes</returns>
        [SecurityCritical]
        public byte[] GetBytes() => this.Decrypt();

        /// <summary>
        /// Returns the <see cref="SecureString"/> as a <see cref="string"/>
        /// </summary>
        /// <returns>The content of the <see cref="SecureString"/> as a <see cref="string"/></returns>
        [SecurityCritical]
        public string GetString() => Encoding.UTF8.GetString(this.Decrypt());

        /// <summary>
        /// Compares two <see cref="SecureString"/> for equality
        /// </summary>
        /// <param name="b">The second <see cref="SecureString"/> to compare</param>
        /// <returns>Returns true if the <see cref="SecureString"/> s are equal; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="b"/> is null</exception>
        public bool IsEqualTo(SecureString b)
        {
            // This is meant to mimic the SecureString extension for Desktop

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            int lengthA = this.data.Length;
            int lengthB = b.data.Length;

            if (lengthA == lengthB)
            {
                for (int x = 0; x < lengthA; ++x)
                    if (this.data[x] != b.data[x])
                        return false;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (!disposeManaged)
            {
                var gc = GCHandle.Alloc(this.data, GCHandleType.Pinned);
                gc.FillWithRandomValues(this.data.Length);
            }
        }

        private byte[] Decrypt()
        {
            var passPhrase = Encoding.UTF8.GetBytes(Debug.HardwareIdentifier.GetHash(HashAlgorithms.Sha512));
            var keyMaterial = CryptographicBuffer.CreateFromByteArray(passPhrase);

            SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            var symetricKey = aes.CreateSymmetricKey(keyMaterial);
            var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, this.data.AsBuffer(), this.initialisationVector.AsBuffer());

            byte[] result;

            CryptographicBuffer.CopyToByteArray(buffDecrypted, out result);

            return result;
        }
    }
}