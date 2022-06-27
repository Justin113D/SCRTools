using SCR.Tools.Dialog.Editor.Viewmodeling;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Colors = SCR.Tools.WPF.Styling.Colors;

namespace SCR.Tools.Dialog.Editor.WPF
{
    public class PortraitExistsConverter : IMultiValueConverter
    {
        private static readonly SolidColorBrush Green = new(Colors.Green);
        private static readonly SolidColorBrush Transparent = new(System.Windows.Media.Colors.Transparent);
        private static readonly SolidColorBrush Red = new(Colors.Red);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string? folder = (string?)values[0];
            VmNodeOption<Color>? character = (VmNodeOption<Color>?)values[1];
            VmNodeOption<Color>? expression = (VmNodeOption<Color>?)values[2];

            if (folder == null || character == null || expression == null)
            {
                return Transparent;
            }

            return File.Exists($"{folder}/{character}_{expression}.png")
                ? Green
                : Red;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
