using System;
using System.Windows;

namespace Cauldron
{
    public partial interface IUtils
    {
        /// <summary>
        /// Returns the mouse position on screen
        /// </summary>
        /// <returns>The mouse position coordinates on screen</returns>
        Point GetMousePosition();

        /// <summary>
        /// Gets a string from the dll defined by <paramref name="moduleName"/> text resources.
        /// </summary>
        /// <param name="moduleName">The dll to retrieve the string resources from. e.g. user32.dll, shell32.dll</param>
        /// <param name="index">The id of the text resource.</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="index"/>. Returns null if not found</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="moduleName"/> is empty</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="moduleName"/> is null</exception>
        string GetStringFromModule(string moduleName, uint index);

        /// <summary>
        /// Gets a (assorted) string from dll text resources.
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="key"/>. Returns null if not found</returns>
        string GetStringFromModule(string key);

        /// <summary>
        /// Gets a string from user32.dll text resources.
        /// </summary>
        /// <param name="index">The id of the text resource.</param>
        /// <returns>The text resource string from user32.dll defined by <paramref name="index"/>. Returns null if not found</returns>
        string GetStringFromModule(uint index);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="window">The window whose window procedure will receive the message. </param>
        /// <param name="windowMessage">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        void SendMessage(Window window, WindowsMessages windowMessage, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.
        /// </summary>
        /// <param name="window">The window to get the monitor info from</param>
        /// <param name="lParam">Additional message-specific information</param>
        void WmGetMinMaxInfo(Window window, IntPtr lParam);
    }
}