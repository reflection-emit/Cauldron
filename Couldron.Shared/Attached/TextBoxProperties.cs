#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else

using System.Windows;
using System.Windows.Controls;

#endif

namespace Couldron.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="TextBox"/> control
    /// </summary>
    public static class TextBoxProperties
    {
        #region Dependency Attached Property AlternativeText

        /// <summary>
        /// Identifies the AlternativeText dependency property
        /// </summary>
        public static readonly DependencyProperty AlternativeTextProperty = DependencyProperty.RegisterAttached("AlternativeText", typeof(string), typeof(TextBoxProperties), new PropertyMetadata(""));

        /// <summary>
        /// Gets the value of AlternativeText
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetAlternativeText(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextProperty);
        }

        /// <summary>
        /// Sets the value of the AlternativeText attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetAlternativeText(DependencyObject obj, string value)
        {
            obj.SetValue(AlternativeTextProperty, value);
        }

        #endregion Dependency Attached Property AlternativeText

        #region Dependency Attached Property Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(TextBoxProperties), new PropertyMetadata(""));

        /// <summary>
        /// Gets the value of Header
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        #endregion Dependency Attached Property Header

        #region Dependency Attached Property ButtonsTemplate

        /// <summary>
        /// Identifies the ButtonsTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty ButtonsTemplateProperty = DependencyProperty.RegisterAttached("ButtonsTemplate", typeof(ControlTemplate), typeof(TextBoxProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of ButtonsTemplate
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ControlTemplate GetButtonsTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(ButtonsTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the ButtonsTemplate attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetButtonsTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ButtonsTemplateProperty, value);
        }

        #endregion Dependency Attached Property ButtonsTemplate
    }
}