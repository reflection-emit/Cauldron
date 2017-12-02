using System;
using System.Globalization;
using System.Windows.Data;

namespace Win32_XAML_Validation_Tests
{
    public sealed class TestMultiBindingConverter : IMultiValueConverter
    {
        /*
         The creation of the assembly will fail if our fody add-in has implementation errors
             */

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}