using Cauldron.Core;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

#endif

namespace Cauldron.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="Button"/> control
    /// </summary>
    public static class ButtonProperties
    {
        #region Dependency Attached Property EmbeddedImageKey

        /// <summary>
        /// Identifies the EmbeddedImageKey dependency property
        /// </summary>
        public static readonly DependencyProperty EmbeddedImageKeyProperty = DependencyProperty.RegisterAttached("EmbeddedImageKey", typeof(string), typeof(ButtonProperties), new PropertyMetadata("", OnEmbeddedImageKeyChanged));

        /// <summary>
        /// Gets the value of EmbeddedImageKey
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetEmbeddedImageKey(DependencyObject obj)
        {
            return (string)obj.GetValue(EmbeddedImageKeyProperty);
        }

        /// <summary>
        /// Sets the value of the EmbeddedImageKey attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetEmbeddedImageKey(DependencyObject obj, string value)
        {
            obj.SetValue(EmbeddedImageKeyProperty, value);
        }

        private static void OnEmbeddedImageKeyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SetImage(dependencyObject, EmbeddedImageManager.GetImage(args.NewValue as string));
        }

        #endregion Dependency Attached Property EmbeddedImageKey

        #region Dependency Attached Property Image

        /// <summary>
        /// Identifies the Image dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(BitmapImage), typeof(ButtonProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of Image
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static BitmapImage GetImage(DependencyObject obj)
        {
            return (BitmapImage)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the value of the Image attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetImage(DependencyObject obj, BitmapImage value)
        {
            obj.SetValue(ImageProperty, value);
        }

        #endregion Dependency Attached Property Image

        #region Dependency Attached Property CategoryColor

        /// <summary>
        /// Identifies the CategoryColor dependency property
        /// </summary>
        public static readonly DependencyProperty CategoryColorProperty = DependencyProperty.RegisterAttached("CategoryColor", typeof(Brush), typeof(ButtonProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of CategoryColor
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static Brush GetCategoryColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CategoryColorProperty);
        }

        /// <summary>
        /// Sets the value of the CategoryColor attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetCategoryColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(CategoryColorProperty, value);
        }

        #endregion Dependency Attached Property CategoryColor
    }
}