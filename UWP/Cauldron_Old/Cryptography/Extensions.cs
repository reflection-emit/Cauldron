using System;
using System.Linq;
using System.Security;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class Extensions
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
            if (algorithm == HashAlgorithms.Md5)
            {
                var md5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                var buffer = CryptographicBuffer.ConvertStringToBinary(target, BinaryStringEncoding.Utf8);
                var hashed = md5.HashData(buffer);
                return CryptographicBuffer.EncodeToHexString(hashed);
            }
            else if (algorithm == HashAlgorithms.Sha512 || algorithm == HashAlgorithms.Sha256)
            {
                var sha = HashAlgorithmProvider.OpenAlgorithm(algorithm == HashAlgorithms.Sha512 ? HashAlgorithmNames.Sha256 : HashAlgorithmNames.Sha512);
                var buffer = CryptographicBuffer.ConvertStringToBinary(target, BinaryStringEncoding.Utf8);

                var hashed = sha.HashData(buffer);
                byte[] bytes;

                CryptographicBuffer.CopyToByteArray(hashed, out bytes);
                return Convert.ToBase64String(bytes);
            }
            else
                throw new NotSupportedException("Unsupported hash algorithm");
        }

        /// <summary>
        /// Get the hash representing the string
        /// </summary>
        /// <param name="target">The bytes array to hash</param>
        /// <param name="algorithm">The hash algortihm to use</param>
        /// <returns>The hash value</returns>
        public static string GetHash(this byte[] target, HashAlgorithms algorithm)
        {
            if (algorithm == HashAlgorithms.Md5)
            {
                var md5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                var buffer = CryptographicBuffer.CreateFromByteArray(target);
                var hashed = md5.HashData(buffer);
                return CryptographicBuffer.EncodeToHexString(hashed);
            }
            else if (algorithm == HashAlgorithms.Sha512 || algorithm == HashAlgorithms.Sha256)
            {
                var sha = HashAlgorithmProvider.OpenAlgorithm(algorithm == HashAlgorithms.Sha512 ? HashAlgorithmNames.Sha256 : HashAlgorithmNames.Sha512);
                var buffer = CryptographicBuffer.CreateFromByteArray(target);

                var hashed = sha.HashData(buffer);
                byte[] bytes;

                CryptographicBuffer.CopyToByteArray(hashed, out bytes);
                return Convert.ToBase64String(bytes);
            }
            else
                throw new NotSupportedException("Unsupported hash algorithm");
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
    }
}