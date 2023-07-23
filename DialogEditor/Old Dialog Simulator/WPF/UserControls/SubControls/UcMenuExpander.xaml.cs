using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SCR.Tools.Dialog.Simulator.WPF.UserControls.SubControls
{
    /// <summary>
    /// Interaction logic for UcMenuExpander.xaml
    /// </summary>
    [ContentProperty("InnerContent")]
    public partial class UcMenuExpander : UserControl
    {
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(
                nameof(InnerContent),
                typeof(object),
                typeof(UcMenuExpander));

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(UcMenuExpander));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty ExpandedProperty =
            DependencyProperty.Register(
                nameof(Expanded),
                typeof(bool),
                typeof(UcMenuExpander),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                });

        public bool Expanded
        {
            get => (bool)GetValue(ExpandedProperty);
            set => SetValue(ExpandedProperty, value);
        }


        public UcMenuExpander()
        {
            InitializeComponent();
        }
    }
}
