using DryIoc;
using System.Windows;
using SCRLanguageEditor.Viewmodel;

namespace SCRLanguageEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IContainer CreateCompositionRoot()
        {
            var container = new Container();
            container.Register<VM_Main>(Reuse.Singleton);
            container.Register<MainWindow>();
            return container;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var compRoot = CreateCompositionRoot();
            Current.MainWindow = compRoot.Resolve<MainWindow>();
            Current.MainWindow?.Show();
        }
    }
}
