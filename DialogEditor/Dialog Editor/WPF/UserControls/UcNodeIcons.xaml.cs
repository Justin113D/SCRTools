using Microsoft.Win32;
using SCR.Tools.DialogEditor.Viewmodeling;
using SCR.Tools.Viewmodeling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeIcons.xaml
    /// </summary>
    public partial class UcNodeIcons : UserControl
    {
        public RelayCommand<VmNodeIcon> CmdSelectIcon
            => new(SelectIcon);

        public VmNodeIcons Viewmodel
            => (VmNodeIcons)DataContext;

        public UcNodeIcons()
        {
            InitializeComponent();
        }

        private void SelectIcon(VmNodeIcon icon)
        {
            OpenFileDialog ofd = new()
            {
                Title = "Select image path",
                Filter = "PNG image (*.png)|*.png"
            };

            if (ofd.ShowDialog() == true)
            {
                icon.IconPath = ofd.FileName;
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
