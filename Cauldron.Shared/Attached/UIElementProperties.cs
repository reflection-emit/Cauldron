#if NETFX_CORE

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

#endif

using System.Collections;

namespace Cauldron.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="UIElement"/>s, that can be used to extend the functionalities of the control
    /// </summary>
    public static class UIElementProperties
    {
        #region Dependency Attached Property AlternativeText

        /// <summary>
        /// Identifies the AlternativeText dependency property
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty AlternativeTextProperty = DependencyProperty.RegisterAttached("AlternativeText", typeof(string), typeof(UIElementProperties), new PropertyMetadata(""));

        /// <summary>
        /// Gets the value of AlternativeText
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetAlternativeText(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextProperty);
        }

        /// <summary>
        /// Sets the value of the AlternativeText attached property
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
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
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(UIElementProperties), new PropertyMetadata(""));

        /// <summary>
        /// Gets the value of Header
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property
        /// <para/>
        /// Supported by <see cref="TextBox"/>, <see cref="ComboBox"/>
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
        /// <para/>
        /// Supported by <see cref="TextBox"/>
        /// </summary>
        public static readonly DependencyProperty ButtonsTemplateProperty = DependencyProperty.RegisterAttached("ButtonsTemplate", typeof(ControlTemplate), typeof(UIElementProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of ButtonsTemplate
        /// <para/>
        /// Supported by <see cref="TextBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ControlTemplate GetButtonsTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(ButtonsTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the ButtonsTemplate attached property
        /// <para/>
        /// Supported by <see cref="TextBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetButtonsTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ButtonsTemplateProperty, value);
        }

        #endregion Dependency Attached Property ButtonsTemplate

        #region Dependency Attached Property SelectedItems

        /// <summary>
        /// Gets the value of SelectedItems
        /// <para/>
        /// Supported by <see cref="ComboBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static IEnumerable GetSelectedItems(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the value of the SelectedItems attached property
        /// <para/>
        /// Supported by <see cref="ComboBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSelectedItems(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property
        /// <para/>
        /// Supported by <see cref="ComboBox"/>
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IEnumerable), typeof(UIElementProperties), new PropertyMetadata(null));

        #endregion Dependency Attached Property SelectedItems

#if !NETFX_CORE

        #region Dependency Attached Property MyProperty

        /// <summary>
        /// Gets the value of IsCollapsed
        /// <para/>
        /// Supported by <see cref="GroupBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetIsCollapsed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCollapsedProperty);
        }

        /// <summary>
        /// Sets the value of the IsCollapsed attached property
        /// <para/>
        /// Supported by <see cref="GroupBox"/>
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsCollapsed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCollapsedProperty, value);
        }

        /// <summary>
        /// Identifies the IsCollapsed dependency property
        /// <para/>
        /// Supported by <see cref="GroupBox"/>
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.RegisterAttached("IsCollapsed", typeof(bool), typeof(UIElementProperties), new PropertyMetadata(false));

        #endregion Dependency Attached Property MyProperty

#endif
    }
}