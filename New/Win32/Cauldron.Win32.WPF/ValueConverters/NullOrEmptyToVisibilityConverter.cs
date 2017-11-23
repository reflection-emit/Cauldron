using Cauldron.Core.Extensions;
using System;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;

#endif

namespace Cauldron.XAML.ValueConverters
{
    /// <summary>
    /// Converts a null or empty value of a string to a visibility value
    /// </summary>
    public sealed class NullOrEmptyToVisibilityConverter : ValueConverterBase
    {
        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">
        /// Always throws <see cref="NotImplementedException"/>. This method is not implemented.
        /// </exception>
        public override object OnConvert(object value, Type targetType, object parameter, string language)
        {
            var stringValue = value as string;

            if (parameter?.ToString().ToBool() ?? false)
                return string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            else
                return string.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">
        /// Always throws <see cref="NotImplementedException"/>. This method is not implemented.
        /// </exception>
        public override object OnConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Impossible to do
            throw new NotImplementedException();
        }
    }
}