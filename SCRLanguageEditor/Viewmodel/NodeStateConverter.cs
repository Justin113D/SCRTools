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
                    result = SCRCommon.WpfStyles.Colors.Red;
                    break;
                case 1: // received update, yellow
                    result = SCRCommon.WpfStyles.Colors.Yellow;
                    break;
                case 2: // translated after update, blue
                    result = SCRCommon.WpfStyles.Colors.Blue;
                    break;
                case 3: // translated, green
                    result = SCRCommon.WpfStyles.Colors.Green;
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
