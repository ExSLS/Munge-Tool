using System.Windows;
using MungeTool.Lib.Configuration;

namespace MungeTool.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigurationManager.Load();
            ConfigurationManager.FixupSettings();

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) =>
            MessageBox.Show($"Unhandled exception: {e.Exception}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
