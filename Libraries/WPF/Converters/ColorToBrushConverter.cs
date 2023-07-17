using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(SolidColorBrush))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color dc = (System.Drawing.Color)value;
            return new SolidColorBrush(new Color { A = dc.A, R = dc.R, G = dc.G, B = dc.B });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color mc = ((SolidColorBrush)value).Color;
            return System.Drawing.Color.FromArgb(mc.A, mc.R, mc.G, mc.B);
        }
    }
}
