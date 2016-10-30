using Cauldron.Localization;
using System;

#if WINDOWS_UWP

using Windows.UI.Xaml.Data;

#else

using System.Windows.Data;
using System.Globalization;

#endif

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides a base class for <see cref="Binding"/> value converters.
    /// </summary>
    public abstract class ValueConverterBase : IValueConverter
    {
        /// <summary>
        /// Gets the ISO 639-1 two-letter code for the language of the current System.Globalization.CultureInfo.
        /// </summary>
        /// <returns>The ISO 639-1 two-letter code for the language of the current System.Globalization.CultureInfo.</returns>
        public static string GetLanguage() => Locale.GetCurrentCultureInfo().TwoLetterISOLanguageName;

#if WINDOWS_UWP

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
#if WINDOWS_UWP
            return this.OnConvert(value, targetType, parameter, language);
#else
            return this.OnConvert(value, targetType, parameter, culture.TwoLetterISOLanguageName);
#endif
        }

#if WINDOWS_UWP

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
#if WINDOWS_UWP
            return this.OnConvertBack(value, targetType, parameter, language);
#else
            return this.OnConvertBack(value, targetType, parameter, culture.TwoLetterISOLanguageName);
#endif
        }

        #region Abstract Methods

        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public abstract object OnConvert(object value, Type targetType, object parameter, string language);

        /// <summary>
        /// Occures if a value is converted
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public abstract object OnConvertBack(object value, Type targetType, object parameter, string language);

        #endregion Abstract Methods
    }
}