using System;
using System.Globalization;
using System.Windows.Controls;

namespace DesktopApp.Domain
{
    public class CNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
        {
            return String.IsNullOrWhiteSpace((value ?? String.Empty).ToString())
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }
    }
}
