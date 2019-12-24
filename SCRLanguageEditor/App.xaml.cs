using DryIoc;
using System.Windows;
using SCRLanguageEditor.Viewmodel;
using SCRLanguageEditor.Data;

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
            container.Register<Settings>(Reuse.Singleton);
            container.Register<VM_Settings>(Reuse.Singleton);
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
