using System.IO;
using System.Linq;
using Microsoft.Build.Locator;

namespace MungeTool.Lib.Tests.Helpers
{
    public static class MsBuildHelper
    {
        /// <summary>
        /// Uses MSBuild to build a sln or csproj file
        /// </summary>
        /// <param name="buildFileFullPath">Path to the csproj or sln file</param>
        /// <param name="additionalParameters">List of additional msbuild parameters in format ";a=b;b=c" (must start with ;)</param>
        public static void ExecuteMsBuildCommand(string buildFileFullPath, string additionalParameters) =>
            ProcessHelper.StartProcess(
                Path.Combine(MSBuildLocator.QueryVisualStudioInstances().ToList().FirstOrDefault()?.MSBuildPath ?? "", "msbuild.exe"),
                $"\"{buildFileFullPath}\" /t:Restore;Rebuild /p:Configuration=Release{additionalParameters}");
    }
}
