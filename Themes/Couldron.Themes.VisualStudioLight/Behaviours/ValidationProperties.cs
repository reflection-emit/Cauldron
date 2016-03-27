using System.Windows;

namespace Couldron.Themes.VisualStudio.Behaviours
{
    internal static class ValidationProperties
    {
        #region Dependency Attached Property IsMandatory

        /// <summary>
        /// Identifies the <see cref="IsMandatoryProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.RegisterAttached("IsMandatory", typeof(bool), typeof(ValidationProperties), new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of <see cref="IsMandatoryProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetIsMandatory(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMandatoryProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="IsMandatoryProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsMandatory(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMandatoryProperty, value);
        }

        #endregion Dependency Attached Property IsMandatory
    }
}