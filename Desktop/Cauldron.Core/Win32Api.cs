using Cauldron.Core.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides methods and properties for common functions in the Windows API
    /// </summary>
    public static class Win32Api
    {
        /// <summary>
        /// Tests whether the current user is an elevated administrator.
        /// </summary>
        public static bool IsCurrentUserAnAdministrator
        {
            get
            {
                try
                {
                    using (var user = WindowsIdentity.GetCurrent())
                        return new WindowsPrincipal(user).IsInRole(WindowsBuiltInRole.Administrator);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window.
        /// Keyboard input is directed to the window, and various visual cues are changed for the user.
        /// The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.
        /// </summary>
        /// <param name="hwnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>Returns true if successful; otherwise false</returns>
        /// <exception cref="ArgumentException"><paramref name="hwnd"/> is <see cref="IntPtr.Zero"/></exception>
        public static bool ActivateWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentException("HWND cannot be IntPtr.Zero");

            return UnsafeNative.SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        /// </summary>
        /// <param name="registeredWindowMessage">The registered window message. use <see cref="Win32Api.RegisterWindowMessage(string)"/> to register a message.</param>
        /// <exception cref="ArgumentException">Invalid registered window message</exception>
        /// <exception cref="Win32Exception">Win32 error occures</exception>
        public static void BroadcastMessage(uint registeredWindowMessage) => BroadcastMessage(registeredWindowMessage, IntPtr.Zero, IntPtr.Zero);

        /// <summary>
        /// The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        /// </summary>
        /// <param name="registeredWindowMessage">The registered window message. use <see cref="Win32Api.RegisterWindowMessage(string)"/> to register a message.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <exception cref="ArgumentException">Invalid registered window message</exception>
        /// <exception cref="Win32Exception">Win32 error occures</exception>
        public static void BroadcastMessage(uint registeredWindowMessage, IntPtr lParam) => BroadcastMessage(registeredWindowMessage, IntPtr.Zero, lParam);

        /// <summary>
        /// The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        /// </summary>
        /// <param name="registeredWindowMessage">The registered window message. use <see cref="Win32Api.RegisterWindowMessage(string)"/> to register a message.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <exception cref="ArgumentException">Invalid registered window message</exception>
        /// <exception cref="Win32Exception">Win32 error occures</exception>
        public static void BroadcastMessage(uint registeredWindowMessage, IntPtr wParam, IntPtr lParam)
        {
            if (registeredWindowMessage < 0xC000)
                throw new ArgumentException("Not a valid registered window message");

            if (UnsafeNative.SendMessage(UnsafeNative.HWND_BROADCAST, (int)registeredWindowMessage, wParam, lParam) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

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

        /// <summary>
        /// Retrieves the string sent by <see cref="Win32Api.SendMessage(IntPtr, string)"/>.
        /// </summary>
        /// <param name="message">The message id of the message</param>
        /// <param name="lParam">The lParameter from the message</param>
        /// <returns>Returns null if <paramref name="message"/> is not the correct message id</returns>
        public static string GetMessage(int message, IntPtr lParam)
        {
            if (message == UnsafeNative.WM_COPYDATA)
            {
                try
                {
                    var data = Marshal.PtrToStructure<UnsafeNative.COPYDATASTRUCT>(lParam);
                    return data.lpData;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the mouse position on screen
        /// </summary>
        /// <returns>The mouse position coordinates on screen</returns>
        public static System.Windows.Point GetMousePosition()
        {
            UnsafeNative.Win32Point w32Mouse = new UnsafeNative.Win32Point();
            UnsafeNative.GetCursorPos(ref w32Mouse);
            return new System.Windows.Point(w32Mouse.X, w32Mouse.Y);
        }

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

        /// <summary>
        /// Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.
        /// </summary>
        /// <param name="message">The message to be registered.</param>
        /// <returns>Returns a message identifier in the range 0xC000 through 0xFFFF</returns>
        /// <exception cref="Win32Exception">Win32 error occures</exception>
        public static uint RegisterWindowMessage(string message)
        {
            var result = UnsafeNative.RegisterWindowMessage(message);

            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return result;
        }

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hwnd">The window handle of the sending window</param>
        /// <param name="windowMessage">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public static void SendMessage(IntPtr hwnd, WindowsMessages windowMessage, IntPtr wParam, IntPtr lParam)
        {
            UnsafeNative.SendMessage(hwnd, (int)windowMessage, wParam, lParam);
        }

        /// <summary>
        /// Sends the specified message string to a window.
        /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hwnd">A handle to the window whose window procedure will receive the message.</param>
        /// <param name="message">The message to be sent to the window</param>
        /// <exception cref="Win32Exception">Win32 error occures</exception>
        /// <exception cref="ArgumentException">HWND is <see cref="IntPtr.Zero"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="message"/> is empty</exception>
        public static void SendMessage(IntPtr hwnd, string message)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentException("HWND cannot be IntPtr.Zero");

            if (message == null)
                throw new ArgumentNullException("The message cannot be null");

            if (message == "")
                throw new ArgumentException("The message cannot be empty");

            var messageBytes = Encoding.Default.GetBytes(message); /* ANSII encoding */
            var data = new UnsafeNative.COPYDATASTRUCT
            {
                dwData = (IntPtr)100,
                lpData = message,
                cbData = messageBytes.Length + 1 /* +1 because of 0 termination */
            };

            if (UnsafeNative.SendMessage(hwnd, UnsafeNative.WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Starts the EntryAssembly elevated.
        /// </summary>
        /// <param name="args">The arguments to be passed to the application</param>
        /// <returns>Returns true if successful; otherwise false</returns>
        public static bool StartElevated(params string[] args)
        {
            if (!IsCurrentUserAnAdministrator)
            {
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetEntryAssembly().Location;
                startInfo.Arguments = args != null && args.Length > 0 ? args.Join(" ") : string.Empty;
                startInfo.Verb = "runas";

                Process.Start(startInfo);
                return true;
            }
            else
                return false;
        }
    }
}
