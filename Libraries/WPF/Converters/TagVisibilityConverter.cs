using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class TagVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility v ? v : (object)Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
