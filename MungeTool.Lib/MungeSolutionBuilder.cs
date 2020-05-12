using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MungeTool.Lib.Helpers;
using MungeTool.Lib.Models;

namespace MungeTool.Lib
{
    public class MungeSolutionBuilder
    {
        public enum ProgressType
        {
            CreateSln,
            AddProjectsToSln,
            ConvertPackageRefsToProjectRefs
        }

        private readonly string[] _packagesToIgnore = {};

        private readonly string _solutionFileName;
        private readonly string _solutionFolder;
        private readonly string _solutionFileNameWithoutExtension;

        public MungeSolutionBuilder(string solutionFileName)
        {
            _solutionFileName = solutionFileName;
            _solutionFolder = Path.GetDirectoryName(solutionFileName);
            _solutionFileNameWithoutExtension = Path.GetFileNameWithoutExtension(solutionFileName);
        }

        public void Create(List<ProjectInfo> projects, Action<int, ProgressType> updateProgressCallback, Action<string> statusMessageCallback, IEnumerable<string> codeRootFolders)
        {
            statusMessageCallback?.Invoke("Creating munge.sln...");
            updateProgressCallback?.Invoke(10, ProgressType.CreateSln);
            CreateSln();
            updateProgressCallback?.Invoke(100, ProgressType.CreateSln);

            var n = 0;

            statusMessageCallback?.Invoke("Adding projects to munge.sln...");

            foreach (var project in projects)
            {
                var content = File.ReadAllText(project.ProjectName);

                // Replace existing project references inside our new <something>.munge.csproj so that they point to the <dependency-project>.munge.csproj counterpart
                content = Regex.Replace(content, @"(<ProjectReference.*)\.csproj", "$1.munge.csproj");

                // Add the AssemblyName attribute so that the output assembly doesn't end with ".munge.dll" or ".munge.exe"
                if (!content.Contains("AssemblyName"))
                    content = content.Replace("</TargetFramework>", $"</TargetFramework>\n    <AssemblyName>{Path.GetFileNameWithoutExtension(project.ProjectName)}</AssemblyName>");

                // Add the RootNamespace attribute so that the root namespace doesn't include 'munge' (ie. inferred from the csproj filename)
                if (!content.Contains("RootNamespace"))
                    content = content.Replace("</TargetFramework>", $"</TargetFramework>\n    <RootNamespace>{Path.GetFileNameWithoutExtension(project.ProjectName)}</RootNamespace>");

                // Remove package references for packages we want to ignore in munge builds
                var packagesToIgnore = _packagesToIgnore.ToList();

                foreach (var packageToIgnore in packagesToIgnore)
                    content = Regex.Replace(content, $"^.*PackageReference.*{packageToIgnore}.*$\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                // Remove postbuild task "del Amewod.Core.Services.*" (this postbuild task is related to payload.zip which we don't use for munged builds)
                content = Regex.Replace(content, @"^.*<PostBuildEvent>cd \$\(TargetDir\)\r?\ndel Amewod.Core.Services\.\*<\/PostBuildEvent>\r?\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                // Write back to a new <something>.munge.csproj copy of the csproj file (*.munge.csproj is in .gitignore)
                File.WriteAllText(project.MungeProjectName, content);

                // Add the project to the solution
                AddProject(project.MungeProjectName);

                updateProgressCallback?.Invoke((int)(100m * n / projects.Count), ProgressType.AddProjectsToSln);
                n++;
            }

            updateProgressCallback?.Invoke(100, ProgressType.AddProjectsToSln);

            statusMessageCallback?.Invoke("Converting project references to package references...");

            n = 0;

            foreach (var project in projects)
            {
                PackageToProjectReferenceConverter.Convert(projects, project.MungeProjectName);

                updateProgressCallback?.Invoke((int)(100m * n / projects.Count), ProgressType.ConvertPackageRefsToProjectRefs);
                n++;
            }

            updateProgressCallback?.Invoke(100, ProgressType.ConvertPackageRefsToProjectRefs);

            statusMessageCallback?.Invoke("Munge complete.");
        }

        private void CreateSln() =>
            ProcessRunner.StartProcess("dotnet", $"new sln -n {_solutionFileNameWithoutExtension} --force", _solutionFolder);

        private void AddProject(string csProj) =>
            ProcessRunner.StartProcess("dotnet",
                $"sln {_solutionFileName} add {csProj}",
                Path.GetDirectoryName(_solutionFileName));
    }
}
