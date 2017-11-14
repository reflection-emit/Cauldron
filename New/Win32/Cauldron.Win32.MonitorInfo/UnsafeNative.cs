using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Cauldron
{
    internal static class UnsafeNative
    {
        public delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rect pRect, int dwData);

        public enum DeviceCap : uint
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,
            LOGPIXELSX = 88,
        }

        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }

        [DllImport("user32")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCap nIndex);

        [DllImport("Shcore.dll")]
        public static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        [DllImport("user32")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("User32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, MonitorOptions dwFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved; /* 1 */
            public POINT ptMaxSize; /* 2 */
            public POINT ptMaxPosition; /* 3 */
            public POINT ptMinTrackSize; /* 4 */
            public POINT ptMaxTrackSize; /* 5 */
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x; /* 1 */
            public int y; /* 2 */
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left; /* 1 */
            public int top; /* 2 */
            public int right; /* 3 */
            public int bottom; /* 4 */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO)); /* 1 */
            public RECT rcMonitor = new RECT(); /* 2 */
            public RECT rcWork = new RECT(); /* 3 */
            public int dwFlags = 0; /* 4 */
        }
    }
}