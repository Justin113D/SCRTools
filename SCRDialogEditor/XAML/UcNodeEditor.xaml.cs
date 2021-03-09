using SCRDialogEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for NodeEditor.xaml
    /// </summary>
    public partial class UcNodeEditor : UserControl
    {
        public static readonly DependencyProperty DialogOptionsProperty =
            DependencyProperty.Register(
                "DialogOptions",
                typeof(VmDialogOptions),
                typeof(UcNodeEditor),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((o, e) => { })
                )
            );

        public VmDialogOptions DialogOptions
        {
            get => (VmDialogOptions)GetValue(DialogOptionsProperty);
            set => SetValue(DialogOptionsProperty, value);
        }

        public UcNodeEditor()
        {
            InitializeComponent();
        }


    }
}
