using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF
{
    [ValueConversion(typeof(double), typeof(GridLength))]
    internal class OffsetWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            Thickness padding = (Thickness)values[1];
            bool canExpand = (bool)values[2];

            // we know of the 15 pixels that the node state border takes
            return new GridLength(width - (padding.Left + 15 + (canExpand ? 20 : 0)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
