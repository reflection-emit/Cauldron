using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Cauldron.Core
{
    /// <summary>
    /// Takes care of pinning a <see cref="SecureString"/> in the memory and data convertion
    /// </summary>
    public sealed class SecureStringHandler : DisposableBase
    {
        private IntPtr secureString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureStringHandler"/> class
        /// </summary>
        /// <param name="secureString">The securestring to handle</param>
        /// <exception cref="ArgumentNullException"><paramref name="secureString"/> is null</exception>
        public SecureStringHandler(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            this.secureString = Marshal.SecureStringToGlobalAllocAnsi(secureString);
        }

        /// <summary>
        /// Returns the <see cref="SecureString"/> value as an array of bytes
        /// </summary>
        /// <returns>An array of bytes</returns>
        [SecurityCritical]
        public byte[] ToBytes()
        {
            unsafe
            {
                byte* data = (byte*)this.secureString.ToPointer();
                byte* endOfString = data;
                while (*endOfString++ != 0)
                {
                }

                // Potential security risk
                byte[] dataCopy = new byte[(int)((endOfString - data) - 1)];

                var gc = GCHandle.Alloc(dataCopy, GCHandleType.Pinned);

                for (int i = 0; i < dataCopy.Length; i++)
                    dataCopy[i] = *(data + i);

                var result = dataCopy;
                gc.RandomizeValues(dataCopy.Length);

                return result;
            }
        }

        /// <summary>
        /// Returns the <see cref="SecureString"/> as a <see cref="string"/>
        /// </summary>
        /// <returns>The content of the <see cref="SecureString"/> as a <see cref="string"/></returns>
        [SecurityCritical]
        public override string ToString()
        {
            unsafe
            {
                byte* data = (byte*)this.secureString.ToPointer();
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
                gc.RandomizeValues(dataCopy.Length);
                return result;
            }
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            Marshal.ZeroFreeGlobalAllocAnsi(this.secureString);
        }
    }
}