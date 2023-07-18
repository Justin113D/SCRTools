using SCR.Tools.WPF.Theme;
using System;

namespace SCR.Tools.Dialog.Editor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ThemeApplication
    {
        public override ThemeAppSettings Settings
            => Editor.Properties.Settings.Default;

        public App()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new();
            app.Run();
        }

    }
}
