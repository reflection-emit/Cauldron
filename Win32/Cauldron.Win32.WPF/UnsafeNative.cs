using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Cauldron.XAML
{
    internal static class UnsafeNative
    {
        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        public static unsafe extern int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Extracts an icon from a exe, dll or ico file.
        /// </summary>
        /// <param name="filename">The filename of the resource</param>
        /// <param name="iconIndex">THe index of the icon</param>
        /// <returns>The icon represented by a byte array</returns>
        public static byte[] ExtractAssociatedIcon(string filename, int iconIndex = 0)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if (filename == "")
                throw new ArgumentException("filename is empty");

            unsafe
            {
                IntPtr[] largeIcons = new IntPtr[] { IntPtr.Zero };

                try
                {
                    var count = UnsafeNative.ExtractIconEx(filename, iconIndex, largeIcons, null, 1);

                    if (count > 0 && largeIcons[0] != IntPtr.Zero)
                    {
                        var icon = Icon.FromHandle(largeIcons[0]);

                        using (var stream = new MemoryStream())
                        {
                            icon.ToBitmap().Save(stream, ImageFormat.Png);
                            stream.Seek(0, SeekOrigin.Begin);

                            return stream.ToArray();
                        }
                    }
                    else
                        return null;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    foreach (IntPtr ptr in largeIcons)
                        if (ptr != IntPtr.Zero)
                            UnsafeNative.DestroyIcon(ptr);
                }
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);
    }
}