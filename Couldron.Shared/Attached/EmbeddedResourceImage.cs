using Couldron.Core;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#else

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

#endif

namespace Couldron.Attached
{
    /// <summary>
    /// An attached property that can assign an image source of the <see cref="Image"/>.
    /// <para />
    /// The attached property is also able to assign images to <see cref="ContentControl"/>'s <see cref="ContentControl.Content"/> property
    /// </summary>
    public static class EmbeddedResourceImage
    {
        /// <summary>
        /// Gets or sets the <see cref="ImageKeyProperty"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty ImageKeyProperty =
            DependencyProperty.RegisterAttached("ImageKey", typeof(string), typeof(EmbeddedResourceImage), new PropertyMetadata("", OnImageKeyChanged));

        /// <summary>
        /// Gets the image key of an embedded resource
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property is attached to</param>
        /// <returns>The image key</returns>
        public static string GetImageKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ImageKeyProperty);
        }

        /// <summary>
        /// Sets the image key of an embedded resource
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property is attached to</param>
        /// <param name="value">The new image key</param>
        public static void SetImageKey(DependencyObject obj, string value)
        {
            obj.SetValue(ImageKeyProperty, value);
        }

        private static void OnImageKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var bitmapImage = EmbeddedImageManager.GetImage(args.NewValue as string);

            if (d is Image)
                d.CastTo<Image>().Source = bitmapImage;
            else if (d is ContentControl)
            {
                var image = new Image();
                image.Source = bitmapImage;
                image.Stretch = Stretch.Uniform;
                image.VerticalAlignment = VerticalAlignment.Stretch;
                image.HorizontalAlignment = HorizontalAlignment.Stretch;

                d.CastTo<ContentControl>().Content = image;
            }
        }
    }
}