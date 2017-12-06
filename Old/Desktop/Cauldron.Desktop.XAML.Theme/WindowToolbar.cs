using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides addional properties that can be added to a custom window
    /// </summary>
    public static class WindowToolbar
    {
        #region Dependency Attached Property Template

        /// <summary>
        /// Identifies the <see cref="TemplateProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.RegisterAttached("Template", typeof(ControlTemplate), typeof(WindowToolbar), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of <see cref="TemplateProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ControlTemplate GetTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(TemplateProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="TemplateProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        #endregion Dependency Attached Property Template
    }
}