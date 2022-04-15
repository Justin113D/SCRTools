using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(GridLength))]
    public class WindowWidthConverter : IValueConverter
    {
        public double Percentage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new GridLength((double)value * Percentage, GridUnitType.Pixel);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
