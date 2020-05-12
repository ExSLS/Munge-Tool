using System;
using System.Collections.Generic;
using System.Linq;

namespace MungeTool.Lib.Helpers
{
    class XmlFileHelper
    {
        const string PrivateAssetsTag = "PrivateAssets";
        const string PackageReferenceTag = "PackageReference";
        const string ProjectReferenceTag = "ProjectReference";
        private readonly string[] _lines;

        public XmlFileHelper(string xmlFile)
        {
            _lines = xmlFile.Split('\n');
        }

        /// <summary>
        /// Fix issue while renaming a tag. The opening tag of a multi line block 
        /// has been converted to a single line tag and the closing tag has been
        /// left with the original name.
        /// </summary>
        public void FixOrphanedPrivateAssets()
        {
            var privateAssetsLocations = FindMatchingLineIndex();
            foreach (var i in privateAssetsLocations) CheckContainingTags(i);
        }

        public override string ToString() => string.Join("\n", _lines);

        private IEnumerable<int> FindMatchingLineIndex()
        {
            var indexedLines = _lines.Select((l, i) => (Line: l, Index: i));
            var matchingLines = indexedLines.Where(l => l.Line.Contains(PrivateAssetsTag));
            return matchingLines.Select(l => l.Index);
        }

        private void CheckContainingTags(int index)
        {
            if (!IsOrphanedPrivateAssets(index)) return;
            ConvertSingleLineToMultiLineTag(index - 1);
            RenameTag(index + 1);
        }

        private bool IsOrphanedPrivateAssets(int index)
        {
            // PrivateAssets tags must be enclosed within a ProjectReference or PackageReference block.
            // Therefore there must be lines either side of the index.
            // Throw if we are at or beyond either end of the file.
            if (index <= 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (index >= (_lines.Length - 1)) throw new ArgumentOutOfRangeException(nameof(index));

            // Check if the PrivateAssets belongs to a ProjectReference.
            return _lines[index - 1].Contains(ProjectReferenceTag);
        }

        private void ConvertSingleLineToMultiLineTag(int lineIndex) 
            => _lines[lineIndex] = _lines[lineIndex].Replace(" />", ">");

        private void RenameTag(int lineIndex)
            => _lines[lineIndex] = _lines[lineIndex].Replace($"</{PackageReferenceTag}>", $"</{ProjectReferenceTag}>");
    }
}
