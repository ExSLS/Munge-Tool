using System.Linq;
using CommandLine;
using MungeTool.Lib;
using MungeTool.Lib.Configuration;
using Con = System.Console;

namespace MungeTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManager.Load();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    ConfigurationManager.Config.CodeRootFolders = options.CodeRootFolders.ToArray(); // Override the Config.yaml with the command line argument

                    if (options.GeneratedMungeSlnFile != null)
                        ConfigurationManager.Config.GeneratedMungeSlnFile = options.GeneratedMungeSlnFile;

                    ConfigurationManager.FixupSettings();

                    var generator = new ProjectDependencyCalculator();

                    var projectExclusionList = ConfigurationManager.Config.MainApplications
                        .Single(x => x.Name == options.MainApplication).Exclusions;

                    var data = generator.GetAllDependenciesRequiredForProject(ConfigurationManager.Config.CodeRootFoldersAbsolute.ToList(), options.ExcludeProjectFolders.ToList(), options.MainApplication, options.IncludeTestProjects, projectExclusionList);

                    var builder = new MungeSolutionBuilder(ConfigurationManager.Config.GeneratedMungeSlnFileAbsolute);

                    builder.Create(data, null, Con.WriteLine, options.CodeRootFolders);
                });
        }
    }
}
