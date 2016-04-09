#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else

using System.Windows;
using System.Windows.Data;

#endif

namespace Couldron.Attached
{
    /// <summary>
    /// Provides an attached property required for the validation
    /// </summary>
    public static partial class ValidationProperties
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

        #region Dependency Attached Property Errors

        /// <summary>
        /// Gets the value of Errors
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetErrors(DependencyObject obj)
        {
            return (string)obj.GetValue(ErrorsProperty);
        }

        /// <summary>
        /// Sets the value of the Errors attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetErrors(DependencyObject obj, string value)
        {
            obj.SetValue(ErrorsProperty, value);
        }

        /// <summary>
        /// Identifies the Errors dependency property
        /// </summary>
        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.RegisterAttached("Errors", typeof(string), typeof(ValidationProperties), new PropertyMetadata(null));

        #endregion Dependency Attached Property Errors

        #region Dependency Attached Property HasErrors

        /// <summary>
        /// Gets the value of HasErrors
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetHasErrors(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorsProperty);
        }

        /// <summary>
        /// Sets the value of the HasErrors attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHasErrors(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorsProperty, value);
        }

        /// <summary>
        /// Identifies the HasErrors dependency property
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty = DependencyProperty.RegisterAttached("HasErrors", typeof(bool), typeof(ValidationProperties), new PropertyMetadata(false));

        #endregion Dependency Attached Property HasErrors
    }
}