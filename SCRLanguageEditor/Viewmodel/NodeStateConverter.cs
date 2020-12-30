using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SCRLanguageEditor.Viewmodel
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    class NodeStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color result = Color.FromArgb(0, 1, 1, 1);
            switch((int)value)
            {
                case 0: // not yet translated, Red
                    result = Color.FromRgb(221, 46, 68);
                    break;
                case 1: // received update, yellow
                    result = Color.FromRgb(244, 144, 12);
                    break;
                case 2: // translated after update, blue
                    result = Color.FromRgb(85, 172, 238);
                    break;
                case 3: // translated, green
                    result = Color.FromRgb(120, 177, 89);
                    break;

            }
            return new SolidColorBrush(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
