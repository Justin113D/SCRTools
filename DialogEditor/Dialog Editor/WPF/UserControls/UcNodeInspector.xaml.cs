using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.Viewmodeling;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeInspector.xaml
    /// </summary>
    public partial class UcNodeInspector : UserControl
    {
        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcNodeInspector)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public UcNodeInspector()
        {
            InitializeComponent();
        }
    }
}
