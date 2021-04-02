using SCRCommon.Viewmodels;
using SCRDialogEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for UcDialogEditor.xaml
    /// </summary>
    public partial class UcDialogEditor : UserControl
    {
        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcDialogEditor)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public UcDialogEditor()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ListBox)sender).IsMouseOver)
                CmdFocusNode.Execute(e.AddedItems[0]);
        }
    }
}
