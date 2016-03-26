using Couldron.Core;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;

namespace Couldron
{
    /// <summary>
    /// Provides a collection of utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the application name
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();

                if (entryAssembly == null)
                    return Assembly.GetCallingAssembly().GetName().Name;

                return entryAssembly.GetName().Name;
            }
        }

        /// <summary>
        /// Gets the full path of the application
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                // this will return null in some cases
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

                return Path.GetDirectoryName(entryAssembly.Location);
            }
        }

        /// <summary>
        /// Gets teh application publisher name
        /// </summary>
        public static string ApplicationPublisher
        {
            get
            {
                var company = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                if (string.IsNullOrEmpty(company))
                    return Assembly.GetAssembly(typeof(Utils)).GetCustomAttribute<AssemblyCompanyAttribute>().Company;

                return company;
            }
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all primitiv types with the == operator and as last resort uses <see cref="object.Equals(object, object)"/> to determin equality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public new static bool Equals(object a, object b)
        {
            if (a == null && b == null)
                return true;

            if (a == null) return false;

            if (object.ReferenceEquals(a, b))
                return true;

            var aType = a.GetType();

            if (aType == typeof(string)) return a as string == b as string;
            if (aType == typeof(int)) return (int)a == (int)b;
            if (aType == typeof(uint)) return (uint)a == (uint)b;
            if (aType == typeof(long)) return (long)a == (long)b;
            if (aType == typeof(ulong)) return (ulong)a == (ulong)b;
            if (aType == typeof(byte)) return (byte)a == (byte)b;
            if (aType == typeof(sbyte)) return (sbyte)a == (sbyte)b;
            if (aType == typeof(float)) return (float)a == (float)b;
            if (aType == typeof(double)) return (double)a == (double)b;
            if (aType == typeof(decimal)) return (decimal)a == (decimal)b;
            if (aType == typeof(bool)) return (bool)a == (bool)b;
            if (aType == typeof(char)) return (char)a == (char)b;
            if (aType == typeof(short)) return (short)a == (short)b;
            if (aType == typeof(ushort)) return (ushort)a == (ushort)b;
            if (aType == typeof(IntPtr)) return (IntPtr)a == (IntPtr)b;
            if (aType == typeof(UIntPtr)) return (UIntPtr)a == (UIntPtr)b;
            if (aType == typeof(DateTime)) return (DateTime)a == (DateTime)b;
            if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a == (DateTimeOffset)b;
            if (aType == typeof(Guid)) return (Guid)a == (Guid)b;

            return a.Equals(b);
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
        /// Checks the password's strength
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>Returns <see cref="PasswordScore"/> rating</returns>
        [SecurityCritical]
        public static PasswordScore GetPasswordScore(string password)
        {
            // Origin: http://social.msdn.microsoft.com/Forums/vstudio/en-US/5e3f27d2-49af-410a-85a2-3c47e3f77fb1/how-to-check-for-password-strength
            if (string.IsNullOrEmpty(password))
                return PasswordScore.Blank;

            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;

            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;

            if (password.Length >= 12)
                score++;

            // number only //"^\d+$" if you need to match more than one digit.
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?"))
                score++;

            // both, lower and upper case
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$"))
                score++;

            // ^[A-Z]+$
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]"))
                score++;

            return (PasswordScore)score;
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
            UnsafeNative.WmGetMinMaxInfo(window, window.GetWindowHandle(), lParam);
        }
    }
}