using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.WPF.Theme.Styles
{
    public partial class AppResources : ResourceDictionary
    {
        private Application? _app;
        private TextBox? _focused;

        public Application App
        {
            get => _app ?? Application.Current;
            set => _app = value;
        }

        public double AppFontSize
        {
            get => (double)this[nameof(AppFontSize)];
            set => this[nameof(AppFontSize)] = value;
        }

        public AppResources()
        {
            _app = null;
            InitializeComponent();
        }

        public AppResources(Application app)
        {
            _app = app;
            InitializeComponent();
            if (_app is ThemeApplication themeApp)
            {
                AppFontSize = themeApp.Settings.Fontsize;
            }
        }

        private void OnRedo(object sender, object e)
            => UndoRedoCommand(true);

        private void OnUndo(object sender, object e)
            => UndoRedoCommand(false);

        private void GotFocus(object sender, RoutedEventArgs e)
            => _focused = (TextBox)sender;

        private void UndoRedoCommand(bool redo)
        {
            if (_focused == null)
                return;

            _focused.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Key key = redo ? Key.Y : Key.Z;

            var windows = App.Windows;
            for (int i = 0; i < windows.Count; i++)
            {
                Window wnd = windows[i];
                if (!wnd.IsActive)
                    continue;

                foreach (object? binding in wnd.InputBindings)
                {
                    if (binding is KeyBinding kb
                        && kb.Key == key
                        && kb.Modifiers == ModifierKeys.Control)
                    {
                        kb.Command.Execute(null);
                        return;
                    }
                }
                return;
            }
            throw new InvalidOperationException("No active window found!");

        }
    }
}
