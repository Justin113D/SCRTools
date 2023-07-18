using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling;
using SCR.Tools.UndoRedo;
using SCR.Tools.WPF.Theme;
using System.Windows;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndHelp.xaml
    /// </summary>
    public partial class WndHelp : ThemeWindow
    {
        private static bool _opened;

        public WndHelp()
        {
            CreateDatacontext();
            InitializeComponent();
        }

        private void CreateDatacontext()
        {
            ChangeTracker changeTracker = new();
            ChangeTracker prev = GlobalChangeTracker;
            changeTracker.Use();

            HeaderNode header = new();
            ParentNode parent = new("Grouping", "This is a grouping, containing string and more groupings");
            StringNode node = new("String", "Default Text", 0, "This is a string, which consists of a unique name/key and a text, which is to be translated by you.\n Below this text, you can see its default value")
            {
                NodeValue = "Custom Text"
            };

            parent.AddChildNode(node);
            header.AddChildNode(parent);

            VmProject format = new(header);
            format.ExpandAll();

            DataContext = format;

            prev.Use();
        }

        public void Open()
        {
            if (_opened)
            {
                Focus();
            }
            else
            {
                Show();
            }
        }

        protected override void Close(object sender, RoutedEventArgs e)
        {
            Hide();
            _opened = false;
        }
    }
}
