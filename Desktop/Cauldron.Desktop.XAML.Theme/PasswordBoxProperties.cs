using System.Windows;
using System.Windows.Data;

namespace Cauldron.XAML.Theme
{
    public static class PasswordBoxProperties
    {
        #region Dependency Attached Property Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(PasswordBoxProperties),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        /// <summary>
        /// Gets the value of Header
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        #endregion Dependency Attached Property Header
    }
}