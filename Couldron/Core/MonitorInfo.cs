using System;
using System.Windows;

namespace Couldron.Core
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

                UnsafeNative.MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) => ++monitorCount > 0;

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
                IntPtr monitor = UnsafeNative.MonitorFromPoint(new UnsafeNative.POINT { x = 0, y = 0 }, UnsafeNative.MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

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
        /// Gets the bounds of the monitor that contains the defined window
        /// </summary>
        /// <param name="window">The window displayed by the monitor</param>
        /// <returns>Returns the bounds of the monitor. Returns null if window is not on any monitor.</returns>
        public static Rect? GetMonitorBounds(Window window)
        {
            IntPtr monitor = UnsafeNative.MonitorFromWindow(window.GetWindowHandle(), UnsafeNative.MonitorOptions.MONITOR_DEFAULTTONULL);

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

            return null;
        }
    }
}