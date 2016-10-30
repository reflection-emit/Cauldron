using System.Linq;
using System.Security;

namespace Cauldron.Cryptography
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static class Extensions
    {
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