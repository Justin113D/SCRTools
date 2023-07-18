using Microsoft.Win32;
using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.WPF.Viewmodeling;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeIcons.xaml
    /// </summary>
    public partial class UcNodeIcons : UserControl
    {
        public RelayCommand<VmNodeOption<string>> CmdSelectIcon
            => new(SelectIcon);

        public VmNodeOptions<string> Viewmodel
            => (VmNodeOptions<string>)DataContext;

        public UcNodeIcons()
        {
            InitializeComponent();
        }

        private void SelectIcon(VmNodeOption<string> icon)
        {
            OpenFileDialog ofd = new()
            {
                Title = "Select image path",
                Filter = "PNG image (*.png)|*.png"
            };

            if (ofd.ShowDialog() == true)
            {
                icon.Value = ofd.FileName;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                Viewmodel.AddOption(textbox.Text);
                textbox.Text = "";
            }
        }
    }

}
