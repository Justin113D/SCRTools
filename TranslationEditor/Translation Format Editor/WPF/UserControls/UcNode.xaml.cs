using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNode.xaml
    /// </summary>
    [ContentProperty("InnerContent")]
    public partial class UcNode : UserControl
    {
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(
                nameof(InnerContent),
                typeof(object),
                typeof(UcNode));

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }

        public static readonly DependencyProperty NameWidthProperty =
           DependencyProperty.Register(
               nameof(NameWidth),
               typeof(double),
               typeof(UcNode)
           );

        public double NameWidth
        {
            get => (double)GetValue(NameWidthProperty);
            set => SetValue(NameWidthProperty, value);
        }

        public static readonly DependencyProperty DescriptionWidthProperty =
           DependencyProperty.Register(
               nameof(DescriptionWidth),
               typeof(double),
               typeof(UcNode)
           );

        public double DescriptionWidth
        {
            get => (double)GetValue(DescriptionWidthProperty);
            set => SetValue(DescriptionWidthProperty, value);
        }

        public static readonly DependencyProperty TreePaddingProperty =
           DependencyProperty.Register(
               nameof(TreePadding),
               typeof(Thickness),
               typeof(UcNode)
           );

        public Thickness TreePadding
        {
            get => (Thickness)GetValue(TreePaddingProperty);
            set => SetValue(TreePaddingProperty, value);
        }

        public static readonly DependencyProperty DragHighlightsProperty =
            DependencyProperty.Register(
                nameof(DragHighlights),
                typeof(bool),
                typeof(UcNode));

        public bool DragHighlights
        {
            get => (bool)GetValue(DragHighlightsProperty);
            set => SetValue(DragHighlightsProperty, value);
        }

        private bool _clickCheck;

        private VmNode ViewModel => (VmNode)DataContext;

        public UcNode()
        {
            InitializeComponent();
        }

        private void GrabSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickCheck = true;
        }

        private void GrabSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_clickCheck)
            {
                bool multi = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                if(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ViewModel.SelectRegion(multi);
                }
                else
                {
                    ViewModel.Select(multi);
                }
                _clickCheck = false;
            }
        }

        private void GrabSurface_MouseLeave(object sender, MouseEventArgs e)
        {
            if(_clickCheck)
            {
                _clickCheck = false;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!_clickCheck)
            {
                ViewModel.DeselectAll();
            }
        }

        private void Event_DeselectAll(object sender, RoutedEventArgs e)
        {
            ViewModel.DeselectAll();
        }
    }
}
