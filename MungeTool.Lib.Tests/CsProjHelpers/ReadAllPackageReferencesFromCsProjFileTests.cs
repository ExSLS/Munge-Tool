using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace MungeTool.Lib.Tests.CsProjHelpers
{
    public class ReadAllPackageReferencesFromCsProjFileTests
    {
        [Test]
        public void GivenCsProjWithPackageReferences_ThenReturnListOfThem() =>
            MungeTool.Lib.Helpers.CsProjHelpers.ReadAllPackageReferencesFromCsProjFile(
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ReadAllPackageReferencesFromCsProjFile", "ProjectWithPackageReferences.csproj"))
                .Should()
                .BeEquivalentTo(new string[]
                {
                    "nunit",
                    "Newtonsoft.Json",
                    "Amewod.Core",
                });
    }
}
