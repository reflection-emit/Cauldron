using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace Cauldron
{
    /// <summary>
    /// Provides properties and methods for getting information about the monitor
    /// </summary>
    public static class MonitorInfo
    {
        /// <summary>
        /// Gets the count of the monitor connected to the device
        /// </summary>
        public static int MonitorCount
        {
            get
            {
                int monitorCount = 0;

                bool callback(IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) => ++monitorCount > 0;

                if (UnsafeNative.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
                    return monitorCount;

                return 0;
            }
        }

        /// <summary>
        /// Gets the bounds of the primary monitor
        /// </summary>
        public static Rect PrimaryMonitorBounds
        {
            get
            {
                var monitor = UnsafeNative.MonitorFromPoint(new UnsafeNative.POINT { x = 0, y = 0 }, MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

                if (monitor != IntPtr.Zero)
                {
                    UnsafeNative.MONITORINFO monitorInfo = new UnsafeNative.MONITORINFO();
                    UnsafeNative.GetMonitorInfo(monitor, monitorInfo);
                    UnsafeNative.RECT rcWorkArea = monitorInfo.rcWork;
                    UnsafeNative.RECT rcMonitorArea = monitorInfo.rcMonitor;

                    return new Rect(
                        Math.Abs(rcWorkArea.left - rcMonitorArea.left),
                        Math.Abs(rcWorkArea.top - rcMonitorArea.top),
                        Math.Abs(rcWorkArea.right - rcWorkArea.left),
                        Math.Abs(rcWorkArea.bottom - rcWorkArea.top));
                }

                return new Rect();
            }
        }

        /// <summary>
        /// Returns the orientation of the current view
        /// </summary>
        /// <returns></returns>
        public static ViewOrientation GetCurrentOrientation()
        {
            var process = Process.GetCurrentProcess();
            var rect = GetMonitorBounds(process.MainWindowHandle);

            if (rect.HasValue)
                return rect.Value.Height < rect.Value.Width ? ViewOrientation.Landscape : ViewOrientation.Portrait;

            return ViewOrientation.Landscape;
        }

        /// <summary>
        /// Gets the dpi of the display. If the window is not on any monitor the primary monitor's
        /// dpi is returned instead.
        /// <para/>
        /// On Windows version lower than 8, the dpi is the same for all monitors.
        /// </summary>
        /// <param name="windowHandle">The handle of the window displayed by the monitor</param>
        /// <returns>The dpi of the current display</returns>
        public static MonitorDpi GetDpi(IntPtr windowHandle)
        {
            if ((Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2) || Environment.OSVersion.Version.Major >= 10)
            {
                var monitor = UnsafeNative.MonitorFromWindow(windowHandle, MonitorOptions.MONITOR_DEFAULTTONULL);

                if (monitor == IntPtr.Zero)
                    monitor = UnsafeNative.MonitorFromPoint(new UnsafeNative.POINT { x = 0, y = 0 }, MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

                UnsafeNative.GetDpiForMonitor(monitor, UnsafeNative.DpiType.Effective, out uint dpiX, out uint dpiY);

                return new MonitorDpi { x = dpiX, y = dpiY };
            }
            else
            {
                var desktop = Graphics.FromHwnd(IntPtr.Zero).GetHdc();

                var logicalPixelsx = (uint)UnsafeNative.GetDeviceCaps(desktop, UnsafeNative.DeviceCap.LOGPIXELSX);
                var logicalPixelsy = (uint)UnsafeNative.GetDeviceCaps(desktop, UnsafeNative.DeviceCap.LOGPIXELSY);

                return new MonitorDpi { x = logicalPixelsx, y = logicalPixelsy };
            }
        }

        /// <summary>
        /// Gets the bounds of the monitor that contains the defined window
        /// </summary>
        /// <param name="windowHandle">The handle of the window displayed by the monitor</param>
        /// <returns>
        /// Returns the bounds of the monitor. Returns null if window is not on any monitor.
        /// </returns>
        public static Rect? GetMonitorBounds(IntPtr windowHandle)
        {
            var monitor = UnsafeNative.MonitorFromWindow(windowHandle, MonitorOptions.MONITOR_DEFAULTTONULL);

            if (monitor != IntPtr.Zero)
            {
                UnsafeNative.MONITORINFO monitorInfo = new UnsafeNative.MONITORINFO();
                UnsafeNative.GetMonitorInfo(monitor, monitorInfo);
                UnsafeNative.RECT rcWorkArea = monitorInfo.rcWork;
                UnsafeNative.RECT rcMonitorArea = monitorInfo.rcMonitor;

                return new Rect(rcMonitorArea.left, rcMonitorArea.top, Math.Abs(rcMonitorArea.left - rcMonitorArea.right), Math.Abs(rcMonitorArea.bottom - rcMonitorArea.top));
            }

            return null;
        }

        /// <summary>
        /// Determines if the window is shown in any of the monitors.
        /// </summary>
        /// <param name="windowHandle">The handle of the window displayed by the monitor</param>
        /// <returns>true if the window is displayed on any monitor; otherwise false</returns>
        public static bool WindowIsInAnyMonitor(IntPtr windowHandle) =>
            // If MonitorFromWindow has returned zero, we are sure that the window is not in any of
            // our monitors
            UnsafeNative.MonitorFromWindow(windowHandle, MonitorOptions.MONITOR_DEFAULTTONULL) != IntPtr.Zero;

        /// <summary>
        /// Sent to a window when the size or position of the window is about to change. An
        /// application can use this message to override the window's default maximized size and
        /// position, or its default minimum or maximum tracking size.
        /// </summary>
        /// <param name="windowHandle">The handle of the window displayed by the monitor</param>
        /// <param name="lParam">Additional message-specific information</param>
        public static void WmGetMinMaxInfo(IntPtr windowHandle, IntPtr lParam)
        {
            var mmi = (UnsafeNative.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(UnsafeNative.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            var monitor = UnsafeNative.MonitorFromWindow(windowHandle, MonitorOptions.MONITOR_DEFAULTTONEAREST);

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