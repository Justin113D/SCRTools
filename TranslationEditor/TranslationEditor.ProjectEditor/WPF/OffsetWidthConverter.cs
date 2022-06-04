using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCR.Tools.TranslationEditor.Project.WPF
{
    [ValueConversion(typeof(double), typeof(GridLength))]
    internal class OffsetWidthConverter : Freezable, IValueConverter
    {
        protected override Freezable CreateInstanceCore()
            => new OffsetWidthConverter();


        public static readonly DependencyProperty PaddingProperty
            = DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(OffsetWidthConverter));

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }



        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double width)
                return 0.0;

            return new GridLength(width - Padding.Left);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
