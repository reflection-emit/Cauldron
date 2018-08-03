using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Cauldron
{
#if !NETFX_CORE

    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class ExtensionsSecureString
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
    }

#endif
}