using System.Runtime.InteropServices;
using System.Text;

namespace Cauldron
{
    internal static class UnsafeNative
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

        [DllImport("shlwapi.dll")]
        public static extern bool PathIsNetworkPath(string pszPath);
    }
}