using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SCRLevelCurvePreviewer.XAML
{
    /// <summary>
    /// Interaction logic for UCValueSlider.xaml
    /// </summary>
    public partial class UcValueSlider : UserControl
    {
        #region Properties

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    ""
                )
            );

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty IsFloatProperty =
            DependencyProperty.Register(
                nameof(IsFloat),
                typeof(bool),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    false
                )
            );

        public bool IsFloat
        {
            get => (bool)GetValue(IsFloatProperty);
            set => SetValue(IsFloatProperty, value);
        }

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(
                nameof(LabelWidth),
                typeof(double),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    50d
                )
            );

        public double LabelWidth
        {
            get => (double)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        public static readonly DependencyProperty TextFieldWidthProperty =
            DependencyProperty.Register(
                nameof(TextFieldWidth),
                typeof(double),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    100d
                )
            );

        public double TextFieldWidth
        {
            get => (double)GetValue(TextFieldWidthProperty);
            set => SetValue(TextFieldWidthProperty, value);
        }

        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register(
                nameof(SliderValue),
                typeof(double),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    0d
                )
            );


        public double SliderValue
        {
            get => (double)GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }

        public static readonly DependencyProperty SliderMaxProperty =
            DependencyProperty.Register(
                nameof(SliderMax),
                typeof(double),
                typeof(UcValueSlider),
                new FrameworkPropertyMetadata(
                    0d
                )
            );


        public double SliderMax
        {
            get => (double)GetValue(SliderMaxProperty);
            set => SetValue(SliderMaxProperty, value);
        }

        #endregion

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public UcValueSlider()
        {
            InitializeComponent();
        }

        private void Text_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

    }
}
