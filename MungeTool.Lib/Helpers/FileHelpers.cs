using System;

namespace MungeTool.Lib.Helpers
{
    public static class FileHelpers
    {
        public static string MakeRelative(string filePath, string referencePath) =>
            new Uri(EnsureEndsWithSlash(referencePath)).MakeRelativeUri(new Uri(filePath)).ToString().Replace("/", "\\");

        private static string EnsureEndsWithSlash(string path) =>
            path.EndsWith("/") ? path : $"{path}/";
    }
}
