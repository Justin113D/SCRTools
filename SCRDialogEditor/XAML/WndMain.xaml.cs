using SCRDialogEditor.Viewmodel;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SCRDialogEditor.XAML
{
    public partial class WndMain : SCRCommon.Wpf.Window
    {
        public static readonly DependencyProperty FeedbackColorProperty =
            DependencyProperty.Register(
                nameof(FeedbackColor),
                typeof(Color?),
                typeof(WndMain)
            );

        public Color? FeedbackColor
        {
            get => (Color?)GetValue(FeedbackColorProperty);
            set => SetValue(FeedbackColorProperty, value);
        }

        private static ColorAnimation _colorAnim;

        private static DoubleAnimation _opacityAnim;

        static WndMain()
        {
            _colorAnim = new()
            {
                To = Colors.Transparent,
                Duration = new(TimeSpan.FromSeconds(0.5d))
            };

            _opacityAnim = new(1, 0, new(TimeSpan.FromSeconds(1)))
            {
                BeginTime = TimeSpan.FromSeconds(2)
            };
        }

        public WndMain()
        {
            DataContext = new VmMain();
            InitializeComponent();
            SetBinding(FeedbackColorProperty, "FeedbackColor");
            FeedbackBackground.Background = new SolidColorBrush(Colors.Transparent);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if(e.Property == FeedbackColorProperty)
            {
                if(e.NewValue == null)
                    return;
                FeedbackAnimation((Color)e.NewValue);
            }
        }

        private void FeedbackAnimation(Color bg)
        {
            // background
            _colorAnim.From = bg;
            FeedbackBackground.Background.BeginAnimation(SolidColorBrush.ColorProperty, _colorAnim, HandoffBehavior.SnapshotAndReplace);

            // text opacity
            Feedback.BeginAnimation(OpacityProperty, null);
            Feedback.Opacity = 1;
            Feedback.BeginAnimation(OpacityProperty, _opacityAnim);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new WndSettings().ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new WndDialogOptions(((VmMain)DataContext).DialogOptions).ShowDialog();
        }
    }
}
