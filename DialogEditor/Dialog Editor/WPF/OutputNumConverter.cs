using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WMColors = System.Windows.Media.Colors;
using Colors = SCR.Tools.WPF.Styling.Colors;

namespace SCR.Tools.DialogEditor.WPF
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class OutputNumConverter : IValueConverter
    {
        private static readonly SolidColorBrush[] Brushes = new SolidColorBrush[]
        {
            new(WMColors.Transparent),
            new(Colors.Blue),
            new(Colors.Yellow),
            new(Colors.Green),
            new(Colors.Red),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
