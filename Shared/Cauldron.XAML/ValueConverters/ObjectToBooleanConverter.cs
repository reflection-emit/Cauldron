using Cauldron.Activator;
using Cauldron.Core.Extensions;
using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows;

#endif

namespace Cauldron.XAML.ValueConverters
{
    /// <summary>
    /// Checks if an object is null. If the object is null, the <see cref="IValueConverter"/> will
    /// return <see cref="Visibility.Collapsed"/>
    /// </summary>
    public sealed class ObjectToBooleanConverter : ValueConverterBase
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
            var p = parameter?.ToString().ToBool();

            if (p.HasValue && p.Value)
                return value == null ? false : true; // Leave it this way, so that it is easy to understand
            else
                return value == null ? true : false; // Leave it this way, so that it is easy to understand
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
            var p = parameter?.ToString().ToBool();

            if (p.HasValue && p.Value)
                return (bool)value == false ? null : Factory.Create(targetType); // Leave it this way, so that it is easy to understand
            else
                return (bool)value == true ? null : Factory.Create(targetType); // Leave it this way, so that it is easy to understand
        }
    }
}