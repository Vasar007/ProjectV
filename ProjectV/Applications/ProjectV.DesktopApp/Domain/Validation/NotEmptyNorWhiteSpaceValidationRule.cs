using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectV.DesktopApp.Domain.Validation
{
    internal sealed class NotEmptyNorWhiteSpaceValidationRule : ValidationRule
    {
        public NotEmptyNorWhiteSpaceValidationRule()
        {
        }

        #region ValidationRule Overridden Methods

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is string str))
            {
                if (!(value is BindingExpression bindingExpression) ||
                    !bindingExpression.TryEvaluate(out str))
                {
                    return new ValidationResult(false, "Content value is not a string.");
                }
            }

            if (str.Length == 0)
            {
                return new ValidationResult(false, "Field is empty.");
            }

            if (str.All(char.IsWhiteSpace))
            {
                return new ValidationResult(false, "Field contains only whitespaces.");
            }

            return ValidationResult.ValidResult;
        }

        #endregion
    }
}
