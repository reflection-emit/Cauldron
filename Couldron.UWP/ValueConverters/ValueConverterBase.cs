using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Couldron.ValueConverters
{
    /// <summary>
    /// Provides a base class for <see cref="Binding"/> value converters.
    /// </summary>
    /// <typeparam name="TInput">The <see cref="Type"/> of the value produced by the binding source</typeparam>
    /// <typeparam name="TOutput">The <see cref="Type"/> of the value produced by the binding target</typeparam>
    public abstract class ValueConverterBase<TInput, TOutput> : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.OnConvert((TInput)value, parameter, new CultureInfo(language));
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter</param>
        /// <returns>A converted value.If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.OnConvertBack((TOutput)value, parameter, new CultureInfo(language));
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected abstract TOutput OnConvert(TInput value, object parameter, CultureInfo culture);

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected abstract TInput OnConvertBack(TOutput value, object parameter, CultureInfo culture);
    }
}