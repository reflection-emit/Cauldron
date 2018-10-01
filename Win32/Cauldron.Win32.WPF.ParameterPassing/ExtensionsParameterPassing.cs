using Cauldron.XAML;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Cauldron
{
    /// <summary>
    /// Provides extension methods
    /// </summary>
    public static class ExtensionsParameterPassing
    {
        /// <summary>
        /// Adds a message hook to the window that handles the parameter passing callback.
        /// </summary>
        /// <param name="window">The window to add the hook to</param>
        public static void AddHookParameterPassing(this Window window)
        {
            if (ParamPassing.@params == null)
                throw new Exception("Execute App.Configure first before adding the hook.");

            if (window == null)
                throw new ArgumentNullException($"{nameof(window)} cannot be null.");

            HwndSource.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle)?
                .AddHook(new HwndSourceHook(HandleMessages));
        }

        private static IntPtr HandleMessages(IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            var data = UnsafeNative.GetMessage(message, lParameter);

            if (data != null)
            {
                if (Application.Current.MainWindow == null)
                    return IntPtr.Zero;

                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;

                if (ParamPassing.@params.BringToFront)
                    UnsafeNative.ActivateWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle);

                var args = data.Split(ParamPassing.@params.DataSeparator);
                ParamPassing.@params.ParameterPassedCallback(args);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}