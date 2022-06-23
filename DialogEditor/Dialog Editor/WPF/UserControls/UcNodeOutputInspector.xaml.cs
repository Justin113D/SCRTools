using SCR.Tools.DialogEditor.Viewmodeling;
using SCR.Tools.Viewmodeling;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeOutputInspector.xaml
    /// </summary>
    public partial class UcNodeOutputInspector : UserControl
    {
        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcNodeOutputInspector)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public UcNodeOutputInspector()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IconSelection.SelectedItem = null;
        }
    }
}
