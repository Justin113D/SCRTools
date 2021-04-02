using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCRDialogEditor
{
    public partial class App : Application
    {
        public double AppFontSize
        {
            get => (double)Resources["BaseFontSize"];
            set => Resources["BaseFontSize"] = value;
        }

        private TextBox _focused;

        private void OnRedo(object sender, object e)
            => UndoRedoCommand(Key.Y, ModifierKeys.Control);

        private void OnUndo(object sender, object e)
            => UndoRedoCommand(Key.Z, ModifierKeys.Control);

        private void GotFocus(object sender, RoutedEventArgs e)
            => _focused = (TextBox)sender;

        private void UndoRedoCommand(Key key, ModifierKeys modifiers)
        {
            _focused.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            Window wnd = Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            foreach(var i in wnd.InputBindings)
            {
                if(i is KeyBinding kb
                    && kb.Key == key
                    && kb.Modifiers == modifiers)
                {
                    kb.Command.Execute(null);
                    break;
                }
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppFontSize = SCRDialogEditor.Properties.Settings.Default.Fontsize;
        }
    }
}
