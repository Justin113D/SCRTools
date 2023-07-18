using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF.UserControls
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

        public static readonly DependencyProperty IsExpandedProperty =
           DependencyProperty.Register(
               nameof(IsExpanded),
               typeof(bool),
               typeof(UcNode),
               new FrameworkPropertyMetadata()
               {
                   BindsTwoWayByDefault = true,
               }
           );

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public UcNode()
        {
            InitializeComponent();
        }
    }
}
