using System.Collections.Generic;
using System.IO;
using System.Linq;
using MungeTool.Lib.Helpers;
using MungeTool.Lib.Models;

namespace MungeTool.Lib
{
    public static class PackageToProjectReferenceConverter
    {
        public static void Convert(List<ProjectInfo> allProjectsInSolution, string csprojFilename)
        {
            var packageReferences = CsProjHelpers.ReadAllPackageReferencesFromCsProjFile(csprojFilename);

            var packageReferencesToConvert = packageReferences
                .Join(allProjectsInSolution, p => p.ToLower(), pis => pis.PackageName.ToLower(),
                    (pr, pis) => new {ProjectReference = pr, ProjectPath = pis.MungeProjectName})
                .ToDictionary(k => k.ProjectReference.ToLower(), v => v.ProjectPath);

            var csProjAbsPath = Path.GetFullPath(csprojFilename);

            File.WriteAllText(csprojFilename,
                CsProjHelpers.ChangePackageReferencesToProjectReferences(File.ReadAllText(csProjAbsPath), packageReferencesToConvert, csProjAbsPath));
        }
    }
}
