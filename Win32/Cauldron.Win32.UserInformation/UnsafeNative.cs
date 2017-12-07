using System.Runtime.InteropServices;
using System.Text;

namespace Cauldron.Core
{
    internal static class UnsafeNative
    {
        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, uint whatever, StringBuilder picpath, int maxLength);

        /// <summary>
        /// Gets the path of the user's profile picture
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The full path of the user's profile picture</returns>
        public static string GetUserTilePath(string username)
        {
            try
            {
                var stringBuilder = new StringBuilder(1000);
                UnsafeNative.GetUserTilePath(username, 0x80000000, stringBuilder, stringBuilder.Capacity);
                return stringBuilder.ToString();
            }
            catch
            {
                throw;
            }
        }
    }
}