using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MungeTool.Lib.Helpers
{
    public static class CsProjHelpers
    {
        public static List<string> GetAllProjects(string rootDir) =>
            Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories)
                .Where(x => !x.EndsWith("munge.csproj"))
                .ToList();

        public static List<string> ReadAllPackageReferencesFromCsProjFile(string csProjFile) =>
            Regex.Matches(File.ReadAllText(csProjFile), @"<PackageReference.*Include=""(.*?)""")
                .Cast<Match>()
                .Select(x => x.Groups[1].Value)
                .ToList();

        public static List<string> ReadAllProjectReferencesFromCsProjFile(string csProjFile) =>
            Regex.Matches(File.ReadAllText(csProjFile), @"<ProjectReference.*Include=""(.*?)""")
                .Cast<Match>()
                .Select(x => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(csProjFile) ?? "", x.Groups[1].Value)))
                .Select(GetPackageNameFromCsProj)
                .ToList();

        public static string GetPackageNameFromCsProj(string csProj)
        {
            var match = Regex.Match(File.ReadAllText(csProj), @"<packageid>(.*?)</packageid>", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                Console.WriteLine($"Project with missing packageid: {csProj}");

                var nuspecFile = Directory.GetFiles(Path.GetDirectoryName(csProj), "*.nuspec")
                    .Where(x => !Path.GetFileNameWithoutExtension(x).Contains("Content"))
                    .SingleOrDefault();

                if (nuspecFile != null)
                    match = Regex.Match(File.ReadAllText(nuspecFile), @"<id>(.*?)</id>", RegexOptions.IgnoreCase);
            }

            return match.Groups[1].Value == ""
                ? Path.GetFileNameWithoutExtension(csProj)
                : match.Groups[1].Value;
        }

        /// <summary>
        /// Converts specified package references to project references in a csproj file
        /// </summary>
        /// <param name="csProjFileToEdit">The path to the csproj file</param>
        /// <param name="packageReferencesToConvert">Packages that should be converted if in the csproj file. Key is package name, value is path to project file</param>
        /// <param name="projDir"></param>
        public static string ChangePackageReferencesToProjectReferences(string csProjFileContent, Dictionary<string, string> packageReferencesToConvert, string csProjAbsPath)
        {
            string GetNewStyleCsProjReplacement(Match match) =>
                packageReferencesToConvert.TryGetValue(match.Groups[2].Value.ToLower(), out var projectInfo)
                    ? $@"{match.Groups[1].Value}<ProjectReference Include=""{FileHelpers.MakeRelative(projectInfo, Path.GetDirectoryName(csProjAbsPath))}"" />"
                    : match.Value;

            var afterReplacement = Regex.Replace(csProjFileContent,
                @"^(\s*)<PackageReference.*Include=""(.*?)"".*$",
                GetNewStyleCsProjReplacement,
                RegexOptions.Multiline);

            return FixMultiLineProjectReferences(afterReplacement);
        }

        /// <summary>
        /// Converting Package to Project references fails if the tag is multi line.
        /// The opening tag is rewritten as a single line tag.
        /// This method finds orphaned closing tags and rejoins them with the opening tag.
        /// </summary>
        /// <param name="brokenXml"></param>
        /// <returns></returns>
        public static string FixMultiLineProjectReferences(string brokenXml)
        {
            var fixer = new XmlFileHelper(brokenXml);
            fixer.FixOrphanedPrivateAssets();
            return fixer.ToString();
        }
    }
}
