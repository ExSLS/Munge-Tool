using System.Collections.Generic;
using CommandLine;

namespace MungeTool.Console
{
    public class Options
    {
        [Option('a', "application", Required = true, HelpText = "The main application to create the munge for")]
        public string MainApplication { get; set; }

        [Option('c', "code-root-folders", Required = true, HelpText = "List of all the project's Git root folders. The first MUST contain the application")]
        public IEnumerable<string> CodeRootFolders { get; set; }

        [Option('m', "munge-sln-location", Required = false, HelpText = "Path and filename to save the munge.sln file. This should be added to .gitignore!")]
        public string GeneratedMungeSlnFile { get; set; }

        [Option('e', "exclude-project-folders", Required = false, HelpText = "List of paths to exclude from the dependency walk (can be a substring, eg. '\\IgnoreThisDirectory\\'")]
        public IEnumerable<string> ExcludeProjectFolders { get; set; }

        [Option('t', "include-test-projects", Required = false, HelpText = "Include dependent test projects")]
        public bool IncludeTestProjects { get; set; }
    }
}
