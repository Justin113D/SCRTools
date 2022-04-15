using System;
using System.Globalization;
using System.Windows.Data;

namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrWhiteSpace((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
