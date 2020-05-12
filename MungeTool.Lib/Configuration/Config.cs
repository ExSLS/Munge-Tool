using System.IO;
using System.Linq;
using MungeTool.Lib.Models;

namespace MungeTool.Lib.Configuration
{
    public class Config
    {
        public string[] CodeRootFolders { get; set; }

        public string GeneratedMungeSlnFile { get; set; }

        public RootApplication[] MainApplications { get; set; }

        public string AmewodRootFolderAbsolute => CodeRootFoldersAbsolute[0];

        public string GeneratedMungeSlnFileAbsolute => Path.IsPathRooted(GeneratedMungeSlnFile) ? GeneratedMungeSlnFile : Path.Combine(UserConfig.CodeRootFolder, GeneratedMungeSlnFile);

        public string[] CodeRootFoldersAbsolute => CodeRootFolders.Select(x => Path.IsPathRooted(x) ? x : Path.Combine(UserConfig.CodeRootFolder, x)).ToArray();

        public UserConfig UserConfig { get; set; }
    }
}
