using System.IO;

namespace MungeTool.Desktop.Models
{
    public class CodeRootFolder
    {
        public CodeRootFolder(string fullPath, bool isIncluded, bool isEditable)
        {
            FullPath = fullPath;
            Name = Path.GetFileName(fullPath.TrimEnd('\\'));
            IsIncluded = isIncluded;
            IsEditable = isEditable;
        }

        public string FullPath { get; }
        public string Name { get; }
        public bool IsIncluded { get; set; }
        public bool IsEditable { get; }
    }
}
