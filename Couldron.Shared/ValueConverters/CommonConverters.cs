using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows.Controls;
using System.Windows.Data;

#endif

namespace Couldron.ValueConverters
{
    /// <summary>
    /// Inverts a bool value
    /// </summary>
    public class BooleanInvertConverter : ValueConverterBase<bool, bool>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override bool OnConvert(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override bool OnConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }

    /// <summary>
    /// Converts a <see cref="bool"/> to <see cref="Visibility"/>. If the value is true, the <see cref="IValueConverter"/> will
    /// return either <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Visible"/> depending on the parameter
    /// </summary>
    public class BooleanToVisibilityConverter : ValueConverterBase<bool, Visibility>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override Visibility OnConvert(bool value, object parameter, CultureInfo culture)
        {
            if (parameter.ToBool())
                return value ? Visibility.Collapsed : Visibility.Visible;
            else
                return value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override bool OnConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            if (parameter.ToBool())
                return value == Visibility.Collapsed;
            else
                return value == Visibility.Visible;
        }
    }

    /// <summary>
    /// Checks a collection if it has child elements and return true or false depending on the converter parameters
    /// <para/>
    /// Default is return false if the collection has no elements
    /// </summary>
    public class CollectionHasElementsToBoolConverter : ValueConverterBase<IEnumerable, bool>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override bool OnConvert(IEnumerable value, object parameter, CultureInfo culture)
        {
            if (parameter.ToBool())
                return value.Any() ? false : true;
            else
                return value.Any() ? true : false;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override IEnumerable OnConvertBack(bool value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Tries to get the localized value of the given key
    /// </summary>
    public class KeyToLocalizedStringConverter : ValueConverterBase<string, string>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override string OnConvert(string value, object parameter, CultureInfo culture)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localized = Factory.Create<Localization>();
                var result = localized[value];

                if (result == null)
                {
#if !NETFX_CORE
                    var defaultWindows = Utils.GetStringFromModule(value);
                    if (defaultWindows != null)
                        return defaultWindows;
#endif
                    return value + " *missing*";
                }

                return result;
            }

            return value;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override string OnConvertBack(string value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checks if a string is null or empty. If the string is null or empty, the <see cref="IValueConverter"/> will
    /// return either <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Visible"/> depending on the parameter
    /// </summary>
    public class NullOrEmptyTovisibilityConverter : ValueConverterBase<string, Visibility>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override Visibility OnConvert(string value, object parameter, CultureInfo culture)
        {
            if (parameter.ToBool())
                return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override string OnConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checks if an object is null. If the object is null, the <see cref="IValueConverter"/> will return
    /// either true or false depending on the parameter
    /// <para/>
    /// If the parameter is True then the converter will return true if the object is null, otherwise false
    /// </summary>
    public class ObjectToBooleanConverter : ValueConverterBase<object, bool>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override bool OnConvert(object value, object parameter, CultureInfo culture)
        {
            return parameter.ToBool() ? value == null : value != null;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override object OnConvertBack(bool value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checks if an object is null. If the object is null, the <see cref="IValueConverter"/> will return <see cref="Visibility.Collapsed"/>
    /// </summary>
    public class ObjectToVisibilityConverter : ValueConverterBase<object, Visibility>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override Visibility OnConvert(object value, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override object OnConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts the value of objects to strings based on the formats specified and inserts them into another string.
    /// </summary>
    public class StringFormatConverter : ValueConverterBase<string, string>
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        protected override string OnConvert(string value, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return value;

            if (culture == null)
                return string.Format(parameter.ToString(), value);
            else
                return string.Format(culture.NumberFormat, parameter.ToString(), value);
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">Always throws <see cref="NotImplementedException"/>. This method is not implemented.</exception>
        protected override string OnConvertBack(string value, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}