using System.Windows;

namespace Cauldron.XAML
{
    internal static class PersistentWindowProperties
    {
        #region Dependency Attached Property Height

        /// <summary>
        /// Identifies the Height dependency property
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("Height", typeof(double), typeof(PersistentWindowProperties), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the value of Height
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(HeightProperty);
        }

        /// <summary>
        /// Sets the value of the Height attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHeight(DependencyObject obj, double value)
        {
            obj.SetValue(HeightProperty, value);
        }

        #endregion Dependency Attached Property Height

        #region Dependency Attached Property Width

        /// <summary>
        /// Identifies the Width dependency property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(double), typeof(PersistentWindowProperties), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the value of Width
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WidthProperty);
        }

        /// <summary>
        /// Sets the value of the Width attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WidthProperty, value);
        }

        #endregion Dependency Attached Property Width
    }
}