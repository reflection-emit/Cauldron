using Cauldron.Cryptography;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Get the hash representing the string. The hash algorithm used is <see cref="HashAlgorithms.Md5"/>
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>The hash value</returns>
        public static string GetHash(this string target) => target.GetHash(HashAlgorithms.Md5);

        /// <summary>
        /// Get the hash representing the string
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <param name="algorithm">The hash algortihm to use</param>
        /// <returns>The hash value</returns>
        public static string GetHash(this string target, HashAlgorithms algorithm)
        {
#if NETCORE
            return UTF8Encoding.UTF8.GetBytes(target).GetHash(algorithm);

#else
            return UTF8Encoding.UTF8.GetBytes(target).GetHash(algorithm);

#endif
        }

        /// <summary>
        /// Get the hash representing the string
        /// </summary>
        /// <param name="target">The bytes array to hash</param>
        /// <param name="algorithm">The hash algortihm to use</param>
        /// <returns>The hash value</returns>
        public static string GetHash(this byte[] target, HashAlgorithms algorithm)
        {
#if NETCORE || NETFX_CORE

            if (algorithm == HashAlgorithms.Md5)
                using (var md5 = MD5.Create())
                    return BitConverter.ToString(md5.ComputeHash(target)).Replace("-", "");
            else if (algorithm == HashAlgorithms.Sha256)
                using (var sha = SHA256.Create())
                    return Convert.ToBase64String(sha.ComputeHash(target));
            else if (algorithm == HashAlgorithms.Sha512)
                using (var sha = SHA512.Create())
                    return Convert.ToBase64String(sha.ComputeHash(target));
            else
                throw new NotSupportedException("Unsupported hash algorithm");
#else
            if (algorithm == HashAlgorithms.Md5)
                using (var md5 = new MD5CryptoServiceProvider())
                    return BitConverter.ToString(md5.ComputeHash(target)).Replace("-", "");
            else if (algorithm == HashAlgorithms.Sha256)
                using (var sha = SHA256.Create())
                    return Convert.ToBase64String(sha.ComputeHash(target));
            else if (algorithm == HashAlgorithms.Sha512)
                using (var sha = SHA512.Create())
                    return Convert.ToBase64String(sha.ComputeHash(target));
            else
                throw new NotSupportedException("Unsupported hash algorithm");

#endif
        }

        internal static byte[] GetBytes(this byte[] target, uint length)
        {
            if (length >= target.Length)
                return target;

            var value = new byte[length];

            Array.Copy(target, value, (int)length);
            return value;
        }

        /// <summary>
        /// Converts a string to a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The <see cref="SecureString"/> equivalent of the string</returns>
        [SecurityCritical]
        public static SecureString ToSecureString(this string value)
        {
            unsafe
            {
                fixed (char* chr = value)
                {
                    return new SecureString(chr, value.Length);
                }
            }
        }

        /// <summary>
        /// Converts a byte array to a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value">The byte array to convert</param>
        /// <returns>The <see cref="SecureString"/> equivalent of the byte array</returns>
        [SecurityCritical]
        public static SecureString ToSecureString(this byte[] value)
        {
            unsafe
            {
                fixed (char* chr = value.Cast<char>().ToArray())
                {
                    return new SecureString(chr, value.Length);
                }
            }
        }

        internal static byte[] ToBytes(this int target) => BitConverter.GetBytes(target);

        internal static int ToInteger(this byte[] target) => BitConverter.ToInt32(target, 0);

        internal static byte[] UnzipAsBytes(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var target = new MemoryStream();

                using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                    decompressionStream.CopyTo(target);

                target.Seek(0, SeekOrigin.Begin);
                return target.ToArray();
            }
        }

        internal static byte[] ZipAsBytes(this byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var target = new MemoryStream();

                using (var compressionStream = new GZipStream(target, CompressionLevel.Optimal, true))
                    stream.CopyTo(compressionStream);

                target.Seek(0, SeekOrigin.Begin);
                return target.ToArray();
            }
        }
    }
}