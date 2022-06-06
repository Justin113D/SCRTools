using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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

        private void OnRedo(object sender, object e)
            => UndoRedoCommand(Key.Y, ModifierKeys.Control);

        private void OnUndo(object sender, object e)
            => UndoRedoCommand(Key.Z, ModifierKeys.Control);

        private void GotFocus(object sender, RoutedEventArgs e)
            => _focused = (TextBox)sender;

        private void UndoRedoCommand(Key key, ModifierKeys modifiers)
        {
            if (_focused == null)
                return;

            _focused.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            Window? wnd = Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            if (wnd == null)
            {
                throw new InvalidOperationException("No active window found!");
            }

            foreach (var i in wnd.InputBindings)
            {
                if (i is KeyBinding kb
                    && kb.Key == key
                    && kb.Modifiers == modifiers)
                {
                    kb.Command.Execute(null);
                    break;
                }
            }
        }
    }
}
