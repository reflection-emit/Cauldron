using Cauldron.Activator;
using Cauldron.Localization;
using System;
using System.Globalization;

namespace Cauldron.XAML.ValueConverters
{
    /// <summary>
    /// Tries to get the localized value of the given key
    /// </summary>
    public sealed class KeyToLocalizedStringConverter : ValueConverterBase
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
            if (Factory.HasContract(typeof(ILocalizationSource)) && value != null)
            {
                var key = parameter?.ToString() ?? value as string;

                if (key != null && Locale.Current.Contains(key))
                    return parameter == null ? Locale.Current[key] :
                         string.Format(string.IsNullOrEmpty(language) ? Locale.GetCurrentCultureInfo() : new CultureInfo(language), Locale.Current[key], value);
            }

            return value;
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
            throw new NotImplementedException();
        }
    }
}