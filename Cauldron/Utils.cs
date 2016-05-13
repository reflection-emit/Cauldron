using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Cauldron
{
    /// <summary>
    /// Provides a collection of utility methods
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Get a value that indicates whether any network connection is available.
        /// <para/>
        /// Returns true if a network connection is available, othwise false
        /// </summary>
        public static bool IsNetworkAvailable
        {
            get
            {
                // Origin: http://stackoverflow.com/questions/520347/how-do-i-check-for-a-network-connection by Simon Mourier

                if (!NetworkInterface.GetIsNetworkAvailable())
                    return false;

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // discard because of standard reasons
                    if ((ni.OperationalStatus != OperationalStatus.Up) ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                        continue;

                    // this allow to filter modems, serial, etc.
                    if (ni.Speed < 10000000)
                        continue;

                    // discard virtual cards (virtual box, virtual pc, etc.)
                    if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                        continue;

                    // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                    if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                        continue;

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns the mouse position on screen
        /// </summary>
        /// <returns>The mouse position coordinates on screen</returns>
        public static Point GetMousePosition()
        {
            UnsafeNative.Win32Point w32Mouse = new UnsafeNative.Win32Point();
            UnsafeNative.GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        /// <summary>
        /// Gets a string from the dll defined by <paramref name="moduleName"/> text resources.
        /// </summary>
        /// <param name="moduleName">The dll to retrieve the string resources from. e.g. user32.dll, shell32.dll</param>
        /// <param name="index">The id of the text resource.</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="index"/>. Returns null if not found</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="moduleName"/> is empty</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="moduleName"/> is null</exception>
        public static string GetStringFromModule(string moduleName, uint index)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));

            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentException("Parameter cannot be empty", nameof(moduleName));

            IntPtr libraryHandle = UnsafeNative.GetModuleHandle(moduleName);
            if (libraryHandle != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(1024);
                if (UnsafeNative.LoadString(libraryHandle, index, sb, 1024) > 0)
                    return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// Gets a (assorted) string from dll text resources.
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="key"/>. Returns null if not found</returns>
        public static string GetStringFromModule(string key)
        {
            switch (key)
            {
                case WindowsStrings.OK: return Utils.GetStringFromModule(800);
                case WindowsStrings.Yes: return Utils.GetStringFromModule(805);
                case WindowsStrings.No: return Utils.GetStringFromModule(806);
                case WindowsStrings.Cancel: return Utils.GetStringFromModule(801);
                case WindowsStrings.Minimize: return Utils.GetStringFromModule(900);
                case WindowsStrings.Maximize: return Utils.GetStringFromModule(901);
                case WindowsStrings.RestoreUp: return Utils.GetStringFromModule(902);
                case WindowsStrings.RestoreDown: return Utils.GetStringFromModule(903);
                case WindowsStrings.Help: return Utils.GetStringFromModule(904);
                case WindowsStrings.Close: return Utils.GetStringFromModule(905);
                case WindowsStrings.Abort: return Utils.GetStringFromModule(802);
                case WindowsStrings.Retry: return Utils.GetStringFromModule(803);
                case WindowsStrings.Continue: return Utils.GetStringFromModule(810);
                case WindowsStrings.Ignore: return Utils.GetStringFromModule(804);
                case WindowsStrings.Error: return Utils.GetStringFromModule(2);

                case WindowsStrings.Copy: return Utils.GetStringFromModule("shell32.dll", 4146);
                case WindowsStrings.Move: return Utils.GetStringFromModule("shell32.dll", 4145);
                case WindowsStrings.Delete: return Utils.GetStringFromModule("shell32.dll", 4147);
                case WindowsStrings.Rename: return Utils.GetStringFromModule("shell32.dll", 4148);
                case WindowsStrings.New: return Utils.GetStringFromModule("shell32.dll", 4151);
                case WindowsStrings.Name: return Utils.GetStringFromModule("shell32.dll", 8976);
                case WindowsStrings.Size: return Utils.GetStringFromModule("shell32.dll", 8978);
                case WindowsStrings.Type: return Utils.GetStringFromModule("shell32.dll", 8979);
                case WindowsStrings.Comments: return Utils.GetStringFromModule("shell32.dll", 8995);
                case WindowsStrings.Open: return Utils.GetStringFromModule("shell32.dll", 12850);
                case WindowsStrings.Execute: return Utils.GetStringFromModule("shell32.dll", 12852);
            }

            return null;
        }

        /// <summary>
        /// Gets a string from user32.dll text resources.
        /// </summary>
        /// <param name="index">The id of the text resource.</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="index"/>. Returns null if not found</returns>
        public static string GetStringFromModule(uint index)
        {
            return GetStringFromModule("user32.dll", index);
        }

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="window">The window whose window procedure will receive the message. </param>
        /// <param name="windowMessage">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public static void SendMessage(Window window, WindowsMessages windowMessage, IntPtr wParam, IntPtr lParam)
        {
            UnsafeNative.SendMessage(window.GetWindowHandle(), (int)windowMessage, wParam, lParam);
        }

        /// <summary>
        /// Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.
        /// </summary>
        /// <param name="window">The window to get the monitor info from</param>
        /// <param name="lParam">Additional message-specific information</param>
        public static void WmGetMinMaxInfo(Window window, IntPtr lParam)
        {
            UnsafeNative.MINMAXINFO mmi = (UnsafeNative.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(UnsafeNative.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            System.IntPtr monitor = UnsafeNative.MonitorFromWindow(window.GetWindowHandle(), UnsafeNative.MonitorOptions.MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {
                UnsafeNative.MONITORINFO monitorInfo = new UnsafeNative.MONITORINFO();
                UnsafeNative.GetMonitorInfo(monitor, monitorInfo);
                UnsafeNative.RECT rcWorkArea = monitorInfo.rcWork;
                UnsafeNative.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}