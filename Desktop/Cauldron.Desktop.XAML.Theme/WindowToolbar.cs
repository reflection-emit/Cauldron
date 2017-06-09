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

        #region Dependency Attached Property MainMenuTemplate

        /// <summary>
        /// Identifies the MainMenuTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty MainMenuTemplateProperty = DependencyProperty.RegisterAttached("MainMenuTemplate", typeof(ControlTemplate), typeof(WindowToolbar), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of MainMenuTemplate
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ControlTemplate GetMainMenuTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(MainMenuTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the MainMenuTemplate attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMainMenuTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(MainMenuTemplateProperty, value);
        }

        #endregion Dependency Attached Property MainMenuTemplate

        #region Dependency Attached Property MainMenuVisibility

        /// <summary>
        /// Identifies the MainMenuVisibility dependency property
        /// </summary>
        public static readonly DependencyProperty MainMenuVisibilityProperty = DependencyProperty.RegisterAttached("MainMenuVisibility", typeof(Visibility), typeof(WindowToolbar), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets the value of MainMenuVisibility
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static Visibility GetMainMenuVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(MainMenuVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the MainMenuVisibility attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMainMenuVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(MainMenuVisibilityProperty, value);
        }

        #endregion Dependency Attached Property MainMenuVisibility
    }
}