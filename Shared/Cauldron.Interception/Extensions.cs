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
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates a new <see cref="Type"/> that implements the properties of an interface defined by <typeparamref name="T"/>
        /// and copies all value of <paramref name="anon"/> to the new object
        /// </summary>
        /// <typeparam name="T">The type of interface to implement</typeparam>
        /// <param name="anon">The anonymous object</param>
        /// <returns>A new object implementing the interface defined by <typeparamref name="T"/></returns>
        public static T CreateType<T>(this object anon) where T : class
        {
            /* NOTE: This will be implemented by Cauldron.Interception.Fody */
            throw new NotImplementedException("No weaving happend.");
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TryDisposeInternal(this object context)
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