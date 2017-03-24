using System;
using System.Text;
using System.ComponentModel;

#if WINDOWS_UWP

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

#else

using System.Security.Cryptography;

#endif

namespace Cauldron.Interception
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Extensions
    {
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TryDispose(this object context)
        {
            var disposable = context as IDisposable;
            disposable?.Dispose();
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetSHA256Hash(this string value)
        {
#if WINDOWS_UWP
            var sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            var buffer = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);

            var hashed = sha.HashData(buffer);
            byte[] bytes;

            CryptographicBuffer.CopyToByteArray(hashed, out bytes);
            return Convert.ToBase64String(bytes);
#elif NETCORE

            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(value)));
#else
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(value)));

#endif
        }
    }
}