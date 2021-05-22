using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Acolyte.Assertions;

namespace ProjectV.DesktopApp.Domain.Converters
{
    internal class BooleansToBooleanInverseConverter : IMultiValueConverter
    {
        public BooleansToBooleanInverseConverter()
        {
        }

        #region IMultiValueConverter Implementation

        public object Convert(object[] values, Type targetType, object parameter,
            CultureInfo culture)
        {
            values.ThrowIfNull(nameof(values));
            targetType.ThrowIfNull(nameof(targetType));

            if (values.Length > 0)
            {
                return values.All(value => IsValid(value));
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException($"{nameof(ConvertBack)} is not implemented.");
        }

        #endregion

        private static bool IsValid(object? value)
        {
            // booleanValue may be Validation.HasError.
            if (value is bool booleanValue) return !booleanValue;

            return !(value is null);
        }
    }
}
