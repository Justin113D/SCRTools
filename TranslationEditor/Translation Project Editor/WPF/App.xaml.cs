using SCR.Tools.WPF.Theme;
using System;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ThemeApplication
    {
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
