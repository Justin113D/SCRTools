using SCR.Tools.WPF.Theme;
using System;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ThemeApplication
    {
        public override ThemeAppSettings Settings 
            => FormatEditor.Properties.Settings.Default;

        public App() : base()
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
