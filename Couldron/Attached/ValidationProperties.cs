using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Couldron.Attached
{
    /// <summary>
    /// Provides an attached property required for the validation
    /// </summary>
    public static partial class ValidationProperties
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

            var frameworkElement = dependencyObject as FrameworkElement;

            if (frameworkElement == null)
                return;

            Func<bool> getErrors = () =>
            {
                var dataContext = frameworkElement.DataContext as INotifyDataErrorInfo;
                if (dataContext == null)
                    return false;

                return !dataContext.GetErrors(propertyName).Any();
            };

            BindingOperations.SetBinding(dependencyObject, FrameworkElement.IsEnabledProperty,
                new Binding
                {
                    Converter = new DisableControlOnValidationErrorConverter(),
                    ConverterParameter = getErrors,
                    Path = new PropertyPath(propertyName)
                });
        }

        private class DisableControlOnValidationErrorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var getErrors = parameter as Func<bool>;
                return getErrors();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Dependency Attached Property IsDisabledByErrorInPropertyName
    }
}