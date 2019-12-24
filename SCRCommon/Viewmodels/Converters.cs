using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace SCRCommon.Viewmodels
{
    [ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
    public class LeftMarginMultiplierConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);

            return new Thickness(Length * item.GetDepth(), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(GridLength))]
    public class WindowWidthConverter : IValueConverter
    {
        public float Percentage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength(Math.Round((double)value * Percentage), GridUnitType.Pixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Window), typeof(int))]
    public class WindowModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                return 1;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
