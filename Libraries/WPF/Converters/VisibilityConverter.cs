using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCR.Tools.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        public Visibility InvisibleType { get; set; } = Visibility.Collapsed;

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Invert ? Visibility.Visible : InvisibleType;

            if (value.GetType() != typeof(bool))
                return Invert ? InvisibleType : Visibility.Visible;

            return (bool)value != Invert ? Visibility.Visible : InvisibleType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
