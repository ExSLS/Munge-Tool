using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MungeTool.Lib.Configuration;
using Squirrel;

namespace MungeTool.Desktop.AutoUpdate
{
    public class AutoUpdater
    {
        public class Results
        {
            public bool HasNewVersion { get; set; }
            public bool FailedToAccessSourcePath { get; set; }
            public string ReleaseNotes { get; set; }
            public string NewVersionNumber { get; set; }
        }

        public async Task<Results> CheckForUpdatesAsync()
        {
            //var autoUpdateLocation = ConfigurationManager.Config.SquirrelAutoUpdateLocation;
            var autoUpdateLocation = string.Empty;

            if (!Directory.Exists(autoUpdateLocation))
                return new Results { HasNewVersion = false, FailedToAccessSourcePath = true };

            using (var mgr = new UpdateManager(autoUpdateLocation))
            {
                var updates = await mgr.CheckForUpdate();

                if (!updates.ReleasesToApply.Any())
                    return new Results { HasNewVersion = false };

                await mgr.DownloadReleases(updates.ReleasesToApply);
                await mgr.ApplyReleases(updates);

                var latestUpdate = updates.ReleasesToApply.OrderBy(x => x.Version).Last();

                return new Results
                {
                    HasNewVersion = true,
                    NewVersionNumber = latestUpdate.Version.ToString(),
                    // ReleaseNotes = latestUpdate.GetReleaseNotes("") TODO: Not sure how this works, and isn't documented
                };
            }
        }
    }
}
