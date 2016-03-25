using System.Windows;
using System.Windows.Controls;

namespace Couldron.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="TextBox"/> control
    /// </summary>
    public static class TextBoxProperties
    {
        #region Dependency Attached Property AlternativeTextKey

        /// <summary>
        /// Identifies the <see cref="AlternativeTextKeyProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty AlternativeTextKeyProperty = DependencyProperty.RegisterAttached("AlternativeTextKey", typeof(string), typeof(TextBoxProperties), new PropertyMetadata("", OnAlternativeTextKeyChanged));

        /// <summary>
        /// Gets the value of <see cref="AlternativeTextKeyProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetAlternativeTextKey(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextKeyProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="AlternativeTextKeyProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetAlternativeTextKey(DependencyObject obj, string value)
        {
            obj.SetValue(AlternativeTextKeyProperty, value);
        }

        private static void OnAlternativeTextKeyChanged(DependencyObject dependencyProperty, DependencyPropertyChangedEventArgs args)
        {
            var localization = Factory.Create<Localization>();
            SetAlternativeTextLocalized(dependencyProperty, localization[args.NewValue as string]);
        }

        #endregion Dependency Attached Property AlternativeTextKey

        #region Dependency Attached Property AlternativeTextLocalized

        /// <summary>
        /// Identifies the <see cref="AlternativeTextLocalizedProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty AlternativeTextLocalizedProperty = DependencyProperty.RegisterAttached("AlternativeTextLocalized", typeof(string), typeof(TextBoxProperties), new PropertyMetadata(""));

        /// <summary>
        /// Gets the value of <see cref="AlternativeTextLocalizedProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetAlternativeTextLocalized(DependencyObject obj)
        {
            return (string)obj.GetValue(AlternativeTextLocalizedProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="AlternativeTextLocalizedProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetAlternativeTextLocalized(DependencyObject obj, string value)
        {
            obj.SetValue(AlternativeTextLocalizedProperty, value);
        }

        #endregion Dependency Attached Property AlternativeTextLocalized
    }
}