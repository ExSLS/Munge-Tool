using System.Collections.Generic;
using System.IO;
using System.Linq;
using MungeTool.Lib.Helpers;
using MungeTool.Lib.Models;

namespace MungeTool.Lib
{
    public class ProjectDependencyCalculator
    {
        public List<ProjectInfo> GetAllDependenciesRequiredForProject(List<string> codeRootsFolders, List<string> excludeProjectFolders, string projectName,
            bool includeTestProjects, IEnumerable<string> projectExclusionList)
        {
            var projects = codeRootsFolders.Where(Directory.Exists).SelectMany(CsProjHelpers.GetAllProjects)
                .Where(x => !projectExclusionList.Any(x.Contains))
                .Where(x => !excludeProjectFolders.Any(x.Contains))
                .Select(x => new ProjectInfo
                {
                    ProjectName = x,
                    PackageName = CsProjHelpers.GetPackageNameFromCsProj(x),
                    PackageReferences = CsProjHelpers.ReadAllPackageReferencesFromCsProjFile(x),
                    ProjectReferences = CsProjHelpers.ReadAllProjectReferencesFromCsProjFile(x),
                }).ToList();

            // Filter package references by projects we know about (ie. not 3rd party ones)
            foreach(var project in projects)
                project.PackageReferences = project.PackageReferences.Where(d => projects.Any(p => p.PackageName.ToLower() == d.ToLower())).ToList();

            var projectLookup = projects.ToDictionary(k => k.PackageName.ToLower(), v => v);

            var results = new List<ProjectInfo>();

            if (!projectLookup.ContainsKey(projectName.ToLower()))
                return results; // This suggests that the 'coderootfolder' specified by the user doesn't contain a git repo with the selected project that they want to munge

            // Recursively scan through all the projects, building up a list of non-3rd-party dependencies
            GetDependenciesRecursively(projectLookup, projectName, includeTestProjects, results);

            return results.Distinct().OrderBy(x => x.PackageName).ToList();
        }

        private void GetDependenciesRecursively(Dictionary<string, ProjectInfo> projectInfo, string projectName, bool includeTestProjects, List<ProjectInfo> results)
        {
            var thisProjInfo = projectInfo[projectName.ToLower()];

            if (thisProjInfo.Visited)
                return;

            thisProjInfo.Visited = true;

            results.Add(thisProjInfo);

            foreach (var dependency in thisProjInfo.PackageReferences)
                GetDependenciesRecursively(projectInfo, dependency, includeTestProjects, results);

            foreach (var dependency in thisProjInfo.ProjectReferences)
                GetDependenciesRecursively(projectInfo, dependency, includeTestProjects, results);

            if (includeTestProjects)
            {
                // For each dependency, check for any 'test' projects that depend on it, and include these in the munge too
                var testProjectsToInclude = projectInfo.Values
                    .Where(x => x.ProjectName.EndsWith(".Tests.csproj") || x.ProjectName.EndsWith(".Test.csproj"))
                    .Where(x => x.PackageReferences.Contains(thisProjInfo.PackageName) || IsProjectNameInProjectReferences(thisProjInfo.ProjectName, x.ProjectReferences))
                    .ToList();

                // Also recurse down from the test project including any of its dependencies (and any test projects depending on those)
                foreach (var testProject in testProjectsToInclude.Where(x => x.ProjectName != thisProjInfo.ProjectName))
                {
                    results.Add(testProject);

                    GetDependenciesRecursively(projectInfo, testProject.PackageName, true, results);
                }
            }
        }

        private bool IsProjectNameInProjectReferences(string projectFullPath, List<string> projectReferences)
        {
            string projectName = Path.GetFileNameWithoutExtension(projectFullPath);
            return projectReferences.Any(projectReference => projectReference.Equals(projectName));
        }
    }
}

