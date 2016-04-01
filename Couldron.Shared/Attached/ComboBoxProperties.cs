using System.Collections;

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
    /// Provides attached properties for the <see cref="ComboBox"/> control
    /// </summary>
    public static class ComboBoxProperties
    {
        #region Dependency Attached Property Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(ComboBoxProperties), new PropertyMetadata(""));

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

        #region Dependency Attached Property AlternativeText

        /// <summary>
        /// Identifies the AlternativeText dependency property
        /// </summary>
        public static readonly DependencyProperty AlternativeTextProperty = DependencyProperty.RegisterAttached("AlternativeText", typeof(string), typeof(ComboBoxProperties), new PropertyMetadata(""));

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

        #region Dependency Attached Property SelectedItems

        /// <summary>
        /// Gets the value of SelectedItems
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static IEnumerable GetSelectedItems(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the value of the SelectedItems attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSelectedItems(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IEnumerable), typeof(ComboBoxProperties), new PropertyMetadata(null));

        #endregion Dependency Attached Property SelectedItems
    }
}