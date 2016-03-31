using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows;

namespace Couldron
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