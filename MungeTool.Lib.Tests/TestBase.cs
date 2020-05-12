using NUnit.Framework;
using System.IO;
using MungeTool.Lib.Configuration;
using MungeTool.Lib.Models;

namespace MungeTool.Lib.Tests
{
    public class TestBase
    {
        [SetUp]
        public void Setup()
        {
            ConfigurationManager.Config = new Config
            {
                CodeRootFolders = new[] {@"MainGitRepo\"},
                MainApplications = new[] {new RootApplication {
                    Name = @"MainApplication",
                    Exclusions = new string[0]
                }},
                GeneratedMungeSlnFile = "Munge.sln",
                UserConfig = new UserConfig {CodeRootFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData")}
            };
        }
    }
}
