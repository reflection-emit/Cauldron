using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides Windows API methods
    /// </summary>
    internal static class UnsafeNative
    {
        public const int GWL_EXSTYLE = (-20);

        public const int SH_SHOW = 5;

        public const int WM_COPYDATA = 0x004A;

        public const int WS_EX_TRANSPARENT = 0x00000020;

        public readonly static IntPtr HWND_BOTTOM = new IntPtr(1);

        public readonly static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        public readonly static IntPtr HWND_MESSAGE = new IntPtr(-3);

        public readonly static IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        public readonly static IntPtr HWND_TOP = new IntPtr(0);

        public readonly static IntPtr HWND_TOPMOST = new IntPtr(-1);

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

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

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        public static unsafe extern int DestroyIcon(IntPtr hIcon);

        [DllImport("user32")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCap nIndex);

        [DllImport("Shcore.dll")]
        public static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        //[DllImport("user32.dll")]
        //public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        /*
         *  Don't remove this so that it won't be forgotten!
         *  Don't use GetModuleHandle ... It is broken for .NET
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        */

        [DllImport("user32")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        //[DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern int GetObject(HandleRef hObject, int nSize, [In, Out] BITMAP bm);

        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, UInt32 whatever, StringBuilder picpath, int maxLength);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("User32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, MonitorOptions dwFlags);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true, EntryPoint = "DeleteObject", CharSet = CharSet.Auto)]
        internal static extern int IntDeleteObject(HandleRef hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;  /* 1 */
            public int cbData;  /* 2 */

            [MarshalAs(UnmanagedType.LPStr)]  /* 3 */
            public string lpData; /* 4 */
        }

        //public struct ICONINFO
        //{
        //    /// <summary>
        //    /// Specifies whether this structure defines an icon or a cursor.
        //    /// A value of TRUE specifies an icon; FALSE specifies a cursor
        //    /// </summary>
        //    public bool fIcon;

        // ///
        // <summary>
        // /// A handle to the icon color bitmap. ///
        // </summary>
        // public IntPtr hbmColor;

        // ///
        // <summary>
        // /// The icon bitmask bitmap ///
        // </summary>
        // public IntPtr hbmMask;

        // ///
        // <summary>
        // /// The x-coordinate of a cursor's hot spot ///
        // </summary>
        // public int xHotspot;

        //    /// <summary>
        //    /// The y-coordinate of a cursor's hot spot
        //    /// </summary>
        //    public int yHotspot;
        //}

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

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int X; /* 1 */
            public int Y; /* 2 */
        };

        //public class BITMAP
        //{
        //    public IntPtr bmBits = IntPtr.Zero;
        //    public short bmBitsPixel = 0;
        //    public int bmHeight = 0;
        //    public short bmPlanes = 0;
        //    public int bmType = 0;
        //    public int bmWidth = 0;
        //    public int bmWidthBytes = 0;
        //}

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO)); /* 1 */
            public RECT rcMonitor = new RECT(); /* 2 */
            public RECT rcWork = new RECT(); /* 3 */
            public int dwFlags = 0; /* 4 */
        }

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);
        }
    }
}