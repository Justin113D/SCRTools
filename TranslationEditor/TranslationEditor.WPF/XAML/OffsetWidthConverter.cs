using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SCR.Tools.TranslationEditor.WPF.XAML
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
