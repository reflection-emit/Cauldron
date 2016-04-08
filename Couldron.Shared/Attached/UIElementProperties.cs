#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else

using System.Windows;
using System.Windows.Media;

#endif

namespace Couldron.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="UIElement"/>s, that can be used to extend the functionalities of the control
    /// </summary>
    public static class UIElementProperties
    {
        #region Dependency Attached Property ColorBrush

        /// <summary>
        /// Gets the value of ColorBrush
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static Brush GetColorBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ColorBrushProperty);
        }

        /// <summary>
        /// Sets the value of the ColorBrush attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetColorBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ColorBrushProperty, value);
        }

        /// <summary>
        /// Identifies the MyProperty dependency property
        /// </summary>
        public static readonly DependencyProperty ColorBrushProperty = DependencyProperty.RegisterAttached("ColorBrush", typeof(Brush), typeof(UIElementProperties), new PropertyMetadata(null));

        #endregion Dependency Attached Property ColorBrush

        #region Dependency Attached Property Boolean

        /// <summary>
        /// Gets the value of Boolean
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetBoolean(DependencyObject obj)
        {
            return (bool)obj.GetValue(BooleanProperty);
        }

        /// <summary>
        /// Sets the value of the Boolean attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetBoolean(DependencyObject obj, bool value)
        {
            obj.SetValue(BooleanProperty, value);
        }

        /// <summary>
        /// Identifies the Boolean dependency property
        /// </summary>
        public static readonly DependencyProperty BooleanProperty = DependencyProperty.RegisterAttached("Boolean", typeof(bool), typeof(UIElementProperties), new PropertyMetadata(false));

        #endregion Dependency Attached Property Boolean
    }
}