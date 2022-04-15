using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SCR.Tools.TranslationEditor.WPF.XAML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new();
            app.Run();
        }

        public App()
        {
            InitializeComponent();
        }
    }
}
