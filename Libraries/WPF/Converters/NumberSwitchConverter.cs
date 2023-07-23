using System;
using System.Globalization;
using System.Windows.Data;

namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class NumberSwitchConverter : IValueConverter
    {
        public double OnFalse { get; set; }

        public double OnTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? OnTrue : OnFalse;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new InvalidOperationException();
    }
}
