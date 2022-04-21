using SCR.Tools.TranslationEditor.WPF.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.TranslationEditor.WPF.XAML.Windows
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        public WndMain()
        {
            DataContext = new VmMain();
            InitializeComponent();
        }

        private void IB_New(object sender, object e)
        {
            MenuBar.NewProject(sender, new());
        }

        private void IB_Open(object sender, object e)
        {
            MenuBar.OpenProject(sender, new());
        }

        private void IB_OpenFormat(object sender, object e)
        {
            MenuBar.LoadFormat(sender, new());
        }

        private void IB_Save(object sender, object e)
        {
            MenuBar.SaveProject(sender, new());
        }

        private void IB_SaveAs(object sender, object e)
        {
            MenuBar.SaveProjectAs(sender, new());
        }
    }
}
