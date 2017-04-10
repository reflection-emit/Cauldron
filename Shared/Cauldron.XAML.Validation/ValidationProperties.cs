using System;
using System.ComponentModel;
using System.Globalization;
using Cauldron.Internal;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

#else

using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Validation
{
    /// <summary>
    /// Provides an attached property required for the validation
    /// </summary>
    public static class ValidationProperties
    {
        #region Dependency Attached Property IsDisabledByErrorInPropertyName

        /// <summary>
        /// Identifies the IsDisabledByErrorInPropertyName dependency property
        /// </summary>
        public static readonly DependencyProperty IsDisabledByErrorInPropertyNameProperty = DependencyProperty.RegisterAttached("IsDisabledByErrorInPropertyName", typeof(string), typeof(ValidationProperties), new PropertyMetadata(null, OnIsDisabledByErrorInPropertyNameChanged));

        /// <summary>
        /// Gets the value of IsDisabledByErrorInPropertyName
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetIsDisabledByErrorInPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(IsDisabledByErrorInPropertyNameProperty);
        }

        /// <summary>
        /// Sets the value of the IsDisabledByErrorInPropertyName attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsDisabledByErrorInPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(IsDisabledByErrorInPropertyNameProperty, value);
        }

        private static void OnIsDisabledByErrorInPropertyNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var propertyName = args.NewValue as string;

            if (string.IsNullOrEmpty(propertyName))
                return;

#if WINDOWS_UWP
            var frameworkElement = dependencyObject as Control;
#else
            var frameworkElement = dependencyObject as FrameworkElement;
#endif

            if (frameworkElement == null)
                return;

            frameworkElement.IsEnabled = false;

            Func<bool> getErrors = () =>
            {
                var dataContext = frameworkElement.DataContext as INotifyDataErrorInfo;
                if (dataContext == null)
                    return false;

                return !dataContext.GetErrors(propertyName).Any_();
            };

#if WINDOWS_UWP
            BindingOperations.SetBinding(dependencyObject, Control.IsEnabledProperty,
#else
            BindingOperations.SetBinding(dependencyObject, FrameworkElement.IsEnabledProperty,
#endif
                new Binding
                {
                    Converter = new DisableControlOnValidationErrorConverter(),
                    ConverterParameter = getErrors,
                    Path = new PropertyPath(propertyName)
                });
        }

        private class DisableControlOnValidationErrorConverter : IValueConverter
        {
#if WINDOWS_UWP

            public object Convert(object value, Type targetType, object parameter, string language)
            {
                var getErrors = parameter as Func<bool>;
                return getErrors();
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new NotImplementedException();
            }

#else

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var getErrors = parameter as Func<bool>;
                return getErrors();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

#endif
        }

        #endregion Dependency Attached Property IsDisabledByErrorInPropertyName

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
        /// Identifies the Errors dependency property
        /// </summary>
        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.RegisterAttached("Errors", typeof(string), typeof(ValidationProperties), new PropertyMetadata(null));

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

        #endregion Dependency Attached Property Errors

        #region Dependency Attached Property HasErrors

        /// <summary>
        /// Identifies the HasErrors dependency property
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty = DependencyProperty.RegisterAttached("HasErrors", typeof(bool), typeof(ValidationProperties), new PropertyMetadata(false));

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

        #endregion Dependency Attached Property HasErrors
    }
}