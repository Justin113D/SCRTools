using SCRCommon.Viewmodels;
using SCRDialogEditor.Viewmodel;
using System.ComponentModel;
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
                nameof(DialogOptions),
                typeof(VmDialogOptions),
                typeof(UcNodeEditor)
            );

        public VmDialogOptions DialogOptions
        {
            get => (VmDialogOptions)GetValue(DialogOptionsProperty);
            set => SetValue(DialogOptionsProperty, value);
        }

        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcNodeEditor)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public UcNodeEditor()
        {
            InitializeComponent();
        }


    }
}
