using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.WPF.Viewmodeling;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcDialogOverview.xaml
    /// </summary>
    public partial class UcDialogOverview : UserControl
    {
        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcDialogOverview)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public UcDialogOverview()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).IsMouseOver)
                CmdFocusNode.Execute(e.AddedItems[0]);
        }
    }
}
