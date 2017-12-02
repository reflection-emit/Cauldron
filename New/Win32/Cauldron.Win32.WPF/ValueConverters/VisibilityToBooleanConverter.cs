using Cauldron.Core.Extensions;
using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows;
using System.Windows.Data;

#endif

namespace Cauldron.XAML.ValueConverters
{
    /// <summary>
    /// Converts a <see cref="bool"/> to <see cref="Visibility"/>. If the value is true, the <see cref="IValueConverter"/> will
    /// return either <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Visible"/> depending on the parameter
    /// </summary>
    public sealed class VisibilityToBooleanConverter : ValueConverterBase
    {
        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public override object OnConvert(object value, Type targetType, object parameter, string language)
        {
            var p = parameter?.ToString().ToBool();

            if (p.HasValue && p.Value)
                return (Visibility)value == Visibility.Collapsed;
            else
                return (Visibility)value == Visibility.Visible;
        }

        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public override object OnConvertBack(object value, Type targetType, object parameter, string language)
        {
            var p = parameter?.ToString().ToBool();
            var v = value?.ToString().ToBool();

            if (p.HasValue && p.Value)
                return v.HasValue && v.Value ? Visibility.Collapsed : Visibility.Visible;
            else
                return v.HasValue && v.Value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}