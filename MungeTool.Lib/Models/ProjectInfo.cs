using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MungeTool.Lib.Models
{
    public class ProjectInfo
    {
        public string ProjectName { get; set; }
        public string PackageName { get; set; }
        public List<string> PackageReferences { get; set; }
        public List<string> ProjectReferences { get; set; }

        public string MungeProjectName => Regex.Replace(ProjectName, @"\\(.*?)\.csproj", "\\$1.munge.csproj");

        /// <summary>
        /// Visited during recursion
        /// </summary>
        public bool Visited { get; set; }
    }
}
