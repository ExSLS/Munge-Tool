#if DEBUG
#else
#define NOT_DEBUG
#endif

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MungeTool.Desktop.AutoUpdate;
using MungeTool.Desktop.ViewModels;
using MungeTool.Lib.Configuration;

namespace MungeTool.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        public MainWindow()
        {
            Instance = this;

            DataContext = new MainWindowViewModel(new WindowFactory(), new MessageFactory());

            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            StartBackgroundCheckForUpdates();

            base.OnContentRendered(e);
        }

        [Conditional("NOT_DEBUG")]
        private void StartBackgroundCheckForUpdates()
        {
            Task.Run(async () =>
            {
                var updateCheckResults = await new AutoUpdater().CheckForUpdatesAsync();

                // NOTE: Squirrel update disabled
                //if (updateCheckResults.FailedToAccessSourcePath)
                //    MessageBox.Show($"Failed to access path: {ConfigurationManager.Config.SquirrelAutoUpdateLocation} to check for new version.\n\nNote that it's possible that you're using an outdated version.");

                if (updateCheckResults.HasNewVersion)
                    new NewVersionPopup(new NewVersionPopupViewModel
                    {
                        NewVersionNumber = updateCheckResults.NewVersionNumber,
                        ReleaseNotes = updateCheckResults.ReleaseNotes,
                    })
                    { Owner = this }.ShowDialog();
            });
        }
    }
}
