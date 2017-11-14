using Cauldron.Core.Extensions;
using Cauldron.Cryptography;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the <see cref="SecureString"/> value as an array of bytes
        /// </summary>
        /// <returns>An array of bytes</returns>
        [SecurityCritical]
        public static byte[] GetBytes(this SecureString secureString)
        {
            unsafe
            {
#if NETCORE

                var secureStringPointer = SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString);
#else
                var secureStringPointer = Marshal.SecureStringToGlobalAllocAnsi(secureString);

#endif

                byte* data = (byte*)secureStringPointer.ToPointer();
                byte* endOfString = data;

                while (*endOfString++ != 0)
                {
                }

                // Potential security risk
                byte[] dataCopy = new byte[(int)((endOfString - data) - 1)];
                var gc = GCHandle.Alloc(dataCopy, GCHandleType.Pinned);

                for (int i = 0; i < dataCopy.Length; i++)
                    dataCopy[i] = *(data + i);

                gc.Free();
                Marshal.ZeroFreeGlobalAllocAnsi(secureStringPointer);

                return dataCopy;
            }
        }

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
#if WINDOWS_UWP
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

#elif NETCORE
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
#if WINDOWS_UWP
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
#elif NETCORE

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

        /// <summary>
        /// Returns the <see cref="SecureString"/> as a <see cref="string"/>
        /// </summary>
        /// <returns>The content of the <see cref="SecureString"/> as a <see cref="string"/></returns>
        [SecurityCritical]
        public static string GetString(this SecureString secureString)
        {
            unsafe
            {
#if NETCORE

                var secureStringPointer = SecureStringMarshal.SecureStringToCoTaskMemUnicode(secureString);
#else
                var secureStringPointer = Marshal.SecureStringToGlobalAllocAnsi(secureString);

#endif

                byte* data = (byte*)secureStringPointer.ToPointer();
                byte* endOfString = data;

                while (*endOfString++ != 0)
                {
                }

                // Potential security risk
                byte[] dataCopy = new byte[(int)((endOfString - data) - 1)];
                var gc = GCHandle.Alloc(dataCopy, GCHandleType.Pinned);

                for (int i = 0; i < dataCopy.Length; i++)
                    dataCopy[i] = *(data + i);

                var result = Encoding.ASCII.GetString(dataCopy);
                gc.FillWithRandomValues(dataCopy.Length);

                Marshal.ZeroFreeGlobalAllocAnsi(secureStringPointer);

                return result;
            }
        }

#if !NETCORE

        /// <summary>
        /// Compares two <see cref="SecureString"/> for equality
        /// </summary>
        /// <param name="a">The first <see cref="SecureString"/> to compare</param>
        /// <param name="b">The second <see cref="SecureString"/> to compare</param>
        /// <returns>Returns true if the <see cref="SecureString"/> s are equal; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="a"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="b"/> is null</exception>
        public static bool IsEqualTo(this SecureString a, SecureString b)
        {
            // Origin: https://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality#23183092
            // Nikola Novak

            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            var bstrA = IntPtr.Zero;
            var bstrB = IntPtr.Zero;

            try
            {
                bstrA = Marshal.SecureStringToBSTR(a);
                bstrB = Marshal.SecureStringToBSTR(b);

                int lengthA = Marshal.ReadInt32(bstrA, -4);
                int lengthB = Marshal.ReadInt32(bstrB, -4);

                if (lengthA == lengthB)
                {
                    for (int x = 0; x < lengthA; ++x)
                        if (Marshal.ReadByte(bstrA, x) != Marshal.ReadByte(bstrB, x))
                            return false;
                }
                else
                    return false;

                return true;
            }
            finally
            {
                if (bstrB != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstrB);

                if (bstrA != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstrA);
            }
        }

#endif

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