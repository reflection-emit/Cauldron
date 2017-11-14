using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Cauldron.WPF.ParameterPassing
{
    internal static class UnsafeNative
    {
        private const int WM_COPYDATA = 0x004A;

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the
        /// window. Keyboard input is directed to the window, and various visual cues are changed for
        /// the user. The system assigns a slightly higher priority to the thread that created the
        /// foreground window than it does to other threads.
        /// </summary>
        /// <param name="hwnd">
        /// A handle to the window that should be activated and brought to the foreground.
        /// </param>
        /// <returns>Returns true if successful; otherwise false</returns>
        /// <exception cref="ArgumentException"><paramref name="hwnd"/> is <see cref="IntPtr.Zero"/></exception>
        public static bool ActivateWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentException("HWND cannot be IntPtr.Zero");

            return UnsafeNative.SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// Retrieves the string sent by <see cref="SendMessage(IntPtr, string)"/>.
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
                    var result = string.Copy(data.lpData);
                    // Marshal.FreeCoTaskMem(lParam); Causes a crash in Release mode
                    return result;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Sends the specified message string to a window. The SendMessage function calls the window
        /// procedure for the specified window and does not return until the window procedure has
        /// processed the message.
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

            if (UnsafeNative.SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;  /* 1 */
            public int cbData;     /* 2 */

            [MarshalAs(UnmanagedType.LPStr)]  /* 3 */
            public string lpData; /* 4 */
        }
    }
}