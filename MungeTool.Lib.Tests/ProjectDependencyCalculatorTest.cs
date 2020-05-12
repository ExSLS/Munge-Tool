using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using MungeTool.Lib.Configuration;
using MungeTool.Lib.Models;

namespace MungeTool.Lib.Tests
{
    public class ProjectDependencyCalculatorTest : TestBase
    {
        [Test]
        public void GetAllDependenciesRequiredForProject_WithoutTests()
        {
            var expectedResults = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    PackageName = "MainApplication",
                    PackageReferences = new List<string> { "MyLibrary" },
                    ProjectName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\MainGitRepo\MainApplication\MainApplication.csproj"),
                    ProjectReferences = new List<string>(),
                    Visited = true,
                },
                new ProjectInfo
                {
                    PackageName = "MyLibrary",
                    PackageReferences = new List<string>(),
                    ProjectName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\MainGitRepo\MyLibrary\MyLibrary.csproj"),
                    ProjectReferences = new List<string>(),
                    Visited = true,
                }
            };

            var results = new ProjectDependencyCalculator().GetAllDependenciesRequiredForProject(
                ConfigurationManager.Config.CodeRootFoldersAbsolute.ToList(),
                excludeProjectFolders: new List<string>(),
                "MainApplication",
                includeTestProjects: false,
                GetProjectExclusionList("MainApplication"));

            results.Should().BeEquivalentTo(expectedResults);
        }

        [Test]
        public void GetAllDependenciesRequiredForProject_WithTests()
        {
            var expectedResults = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    PackageName = "MainApplication",
                    PackageReferences = new List<string> { "MyLibrary" },
                    ProjectName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\MainGitRepo\MainApplication\MainApplication.csproj"),
                    ProjectReferences = new List<string>(),
                    Visited = true,
                },
                new ProjectInfo
                {
                    PackageName = "MyLibrary",
                    PackageReferences = new List<string>(),
                    ProjectName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\MainGitRepo\MyLibrary\MyLibrary.csproj"),
                    ProjectReferences = new List<string>(),
                    Visited = true,
                },
                new ProjectInfo
                {
                    PackageName = "MainApplication.Tests",
                    PackageReferences = new List<string>(),
                    ProjectName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\MainGitRepo\MainApplication.Tests\MainApplication.Tests.csproj"),
                    ProjectReferences = new List<string> { "MainApplication" },
                    Visited = true,
                }
            };

            var results = new ProjectDependencyCalculator().GetAllDependenciesRequiredForProject(
                ConfigurationManager.Config.CodeRootFoldersAbsolute.ToList(),
                excludeProjectFolders: new List<string>(),
                "MainApplication",
                includeTestProjects: true,
                GetProjectExclusionList("MainApplication"));

            results.Should().BeEquivalentTo(expectedResults);
        }

        public static string[] GetProjectExclusionList(string projectName) =>
            ConfigurationManager.Config.MainApplications
                .Single(x => x.Name == projectName).Exclusions;
    }
}
