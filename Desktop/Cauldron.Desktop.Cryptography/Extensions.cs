using Cauldron.Core.Extensions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Cauldron.Cryptography
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