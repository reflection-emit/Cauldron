#if WINDOWS_UWP

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides a wrapper for <see cref="BitmapImage"/> for  Windows.UI.Xaml.Media.Imaging.BitmapImage (UWP) and System.Windows.Media.Imaging.BitmapImage (Desktop)
    /// </summary>
    public sealed class BitmapImageEx
    {
        private BitmapImage bitmapImage;

        /// <summary>
        /// Gets the wrapped <see cref="BitmapImage"/>.
        /// </summary>
        public BitmapImage Value { get { return this.bitmapImage; } }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public static implicit operator BitmapImage(BitmapImageEx bitmapImage) => bitmapImage.bitmapImage;

        public static implicit operator BitmapImageEx(BitmapImage bitmapImage)
        {
            var result = new BitmapImageEx();
            result.bitmapImage = bitmapImage;
            return result;
        }

        public static implicit operator ImageSource(BitmapImageEx bitmapImage) => bitmapImage.bitmapImage;

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}