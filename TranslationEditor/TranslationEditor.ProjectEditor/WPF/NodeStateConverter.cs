using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Colors = SCR.Tools.WPF.Styling.Colors;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    class NodeStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color result = Color.FromArgb(0, 1, 1, 1);
            switch ((int)value)
            {
                case 1: // translated, green
                    result = Colors.Green;
                    break;
                case 2: // Untranslated
                    result = Colors.Yellow;
                    break;
                case 3: // not yet translated, Red
                    result = Colors.Red;
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
