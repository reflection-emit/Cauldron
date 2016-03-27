using Couldron.Core;
using System.Windows;

namespace Couldron.Attached
{
    /// <summary>
    /// An attached property that can assign an image source of the <see cref="System.Windows.Controls.Image"/>.
    /// <para />
    /// The attached property is also able to assign images to <see cref="System.Windows.Controls.ContentControl"/>'s <see cref="System.Windows.Controls.ContentControl.Content"/> property
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

            if (d is System.Windows.Controls.Image)
                d.CastTo<System.Windows.Controls.Image>().Source = bitmapImage;
            else if (d is System.Windows.Controls.ContentControl)
            {
                var image = new System.Windows.Controls.Image();
                image.Source = bitmapImage;
                image.Stretch = System.Windows.Media.Stretch.Uniform;
                image.VerticalAlignment = VerticalAlignment.Stretch;
                image.HorizontalAlignment = HorizontalAlignment.Stretch;

                d.CastTo<System.Windows.Controls.ContentControl>().Content = image;
            }
        }
    }
}