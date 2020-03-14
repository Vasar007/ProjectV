using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ThingAppraiser.DesktopApp.Domain.Validation
{
    internal sealed class NumberValidationRule : ValidationRule
    {
        private readonly NotEmptyNorWhiteSpaceValidationRule _notEmptyNorWhiteSpace;


        public NumberValidationRule()
        {
            _notEmptyNorWhiteSpace = new NotEmptyNorWhiteSpaceValidationRule();
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

            ValidationResult notEmptyNorWhiteSpaceResult =
                _notEmptyNorWhiteSpace.Validate(value, cultureInfo);

            if (!notEmptyNorWhiteSpaceResult.IsValid) return notEmptyNorWhiteSpaceResult;

            int? result = TryParseInteger(str, out string? message);
            if (!result.HasValue)
            {
                message ??= "Field contains illegal characters.";
                return new ValidationResult(false, message);
            }

            if (result.Value <= 0)
            {
                return new ValidationResult(false, "Parameter should be positive.");
            }

            return ValidationResult.ValidResult;
        }

        #endregion

        private static int? TryParseInteger(string value, out string? message)
        {
            int? result = null;

            try
            {
                result = int.Parse(value);
                message = null;
                return result;
            }
            catch (OverflowException)
            {
                message = "The value is outside the range.";
            }
            catch (FormatException)
            {
                message = "The value is not in a recognizable format.";
            }

            return result;
        }
    }
}
