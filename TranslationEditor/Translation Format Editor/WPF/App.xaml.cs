using SCR.Tools.WPF.Theme;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ThemeApplication
    {
        public double AppFontSize
        {
            get => (double)Resources[nameof(AppFontSize)];
            set => Resources[nameof(AppFontSize)] = value;
        }

        private TextBox? _focused;

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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppFontSize = FormatEditor.Properties.Settings.Default.Fontsize;
        }

    }
}
