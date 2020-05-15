using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace MungeTool.Desktop.Validation
{
    public class CodeRootDirExistsValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var codeRootDir = value?.ToString();

            if (codeRootDir == null || !Directory.Exists(codeRootDir))
                return new ValidationResult(false, "Code root directory does not exist");

            return new ValidationResult(true, null);
        }
    }
}
