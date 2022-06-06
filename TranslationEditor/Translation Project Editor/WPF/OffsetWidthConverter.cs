using System;
using System.Globalization;
using System.Windows;
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
            return new GridLength(width - padding.Left);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
