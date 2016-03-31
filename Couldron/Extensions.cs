using Couldron.Behaviours;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Copy and modifies the alpha channel of the <see cref="SolidColorBrush"/>'s <see cref="Color"/>
        /// </summary>
        /// <param name="brush">The Solidcolorbrush to copy the color from</param>
        /// <param name="alpha">The new alpha channel of the <see cref="SolidColorBrush"/></param>
        /// <returns>A new instance of the <see cref="SolidColorBrush"/></returns>
        public static SolidColorBrush ChangeAlpha(this SolidColorBrush brush, byte alpha)
        {
            return new SolidColorBrush(new Color { A = alpha, R = brush.Color.R, G = brush.Color.G, B = brush.Color.B });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// <para/>
        /// This will dispose an object if it implements the <see cref="IDisposable "/> interface.
        /// <para/>
        /// If the object is a <see cref="FrameworkElement"/> it will try to find known diposable attached properties.
        /// <para />
        /// It will also dispose the the <see cref="FrameworkElement.DataContext"/> content.
        /// </summary>
        /// <param name="context">The object to dispose</param>
        public static void DisposeAll(this object context)
        {
            if (context == null)
                return;

            // dispose the diposables
            (context as IDisposable).IsNotNull(x => x.Dispose());
            (context as FrameworkElement).IsNotNull(x =>
            {
                // Dispose the attach behaviours
                foreach (var child in x.GetVisualChildren())
                    child.DisposeAll();

                Interaction.GetBehaviours(x).IsNotNull(o => o.Dispose());

                // Dispose the datacontext
                x.DataContext.DisposeAll();
            });
        }

        /// <summary>
        /// Hashes a string with MD5
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>A string representing the hash of the original stirng</returns>
        public static string GetMD5HashString(this string target)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] textToHash = Encoding.Default.GetBytes(target);
                byte[] result = md5.ComputeHash(textToHash);
                return System.BitConverter.ToString(result);
            }
        }

        /// <summary>
        /// Hashes a string with Sha256
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>An array of bytes that represents the hash</returns>
        public static byte[] GetSha256HashBytes(this string target)
        {
            using (SHA256 sha = SHA256.Create())
                return sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(target));
        }

        /// <summary>
        /// Hashes a string with Sha256
        /// </summary>
        /// <param name="target">The string to hash</param>
        /// <returns>An string that represents the hash of the original string</returns>
        public static string GetSha256HashString(this string target)
        {
            return target.GetSha256HashBytes().ToBase64String();
        }

        /// <summary>
        /// Gets the window handle for a Windows Presentation Foundation (WPF) window
        /// </summary>
        /// <param name="window">A WPF window object.</param>
        /// <returns>The Windows Presentation Foundation (WPF) window handle (HWND).</returns>
        public static IntPtr GetWindowHandle(this Window window)
        {
            return new System.Windows.Interop.WindowInteropHelper(window).Handle;
        }

        /// <summary>
        /// Checks if the type has implemented the defined interface
        /// </summary>
        /// <typeparam name="T">The type of interface to look for</typeparam>
        /// <param name="type">The type that may implements the interface <typeparamref name="T"/></param>
        /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is not an interface</exception>
        /// <returns>True if the <paramref name="type"/> has implemented the interface <typeparamref name="T"/></returns>
        public static bool ImplementsInterface<T>(this Type type)
        {
            var typeOfInterface = typeof(T);

            if (!typeOfInterface.IsInterface)
                throw new ArgumentException("T is not an interface", nameof(T));

            return type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeOfInterface);
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a <see cref="Nullable{T}"/>
        /// </summary>
        /// <param name="target">The type to test</param>
        /// <returns>Returns true if the type is <see cref="Nullable{T}"/></returns>
        public static bool IsNullable(this Type target)
        {
            return target.IsGenericType && Nullable.GetUnderlyingType(target) != null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BitmapImage"/> and assigns the <see cref="Stream"/> to its <see cref="BitmapImage.StreamSource"/> property
        /// <para/>
        /// Returns null if <paramref name="stream"/> is null.
        /// </summary>
        /// <param name="stream">The stream that contains an image</param>
        /// <returns>A new instance of <see cref="BitmapImage"/></returns>
        public static BitmapImage ToBitmapImage(this Stream stream)
        {
            if (stream == null)
                return null;

            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Converts a string to a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The <see cref="SecureString"/> equivalent of the string</returns>
        [SecurityCritical]
        public static SecureString ToSecureString(this string value)
        {
            var result = new SecureString();

            for (int i = 0; i < value.Length; i++)
                result.AppendChar(value[i]);

            return result;
        }
    }
}