#if WINDOWS_UWP

using Cauldron.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

#else

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

namespace Cauldron.XAML.Interactivity.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="Button"/> control
    /// </summary>
    public static class ButtonProperties
    {
        #region Dependency Attached Property Text

        /// <summary>
        /// Identifies the Text dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(ButtonProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of Text
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        /// <summary>
        /// Sets the value of the Text attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        #endregion Dependency Attached Property Text

        #region Dependency Attached Property ImageKey

        /// <summary>
        /// Identifies the ImageKey dependency property
        /// </summary>
        public static readonly DependencyProperty ImageKeyProperty = DependencyProperty.RegisterAttached("ImageKey", typeof(string), typeof(ButtonProperties), new PropertyMetadata(null, OnImageKeyChanged));

        /// <summary>
        /// Gets the value of ImageKey
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetImageKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ImageKeyProperty);
        }

        /// <summary>
        /// Sets the value of the ImageKey attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetImageKey(DependencyObject obj, string value)
        {
            obj.SetValue(ImageKeyProperty, value);
        }

        private static void OnImageKeyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var buttonBase = dependencyObject as ButtonBase;
            var name = args.NewValue as string;

            if (buttonBase == null)
                throw new NotSupportedException($"This attached property can only be attached to a {typeof(ButtonBase).FullName}");

            if (string.IsNullOrEmpty(name))
            {
                buttonBase.Content = null;
                return;
            }

            var image = new Image();
            image.Stretch = Stretch.Uniform;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            image.HorizontalAlignment = HorizontalAlignment.Stretch;

            buttonBase.Content = image;

            ImageProperties.SetImageKey(image, name);
        }

        #endregion Dependency Attached Property ImageKey
    }
}