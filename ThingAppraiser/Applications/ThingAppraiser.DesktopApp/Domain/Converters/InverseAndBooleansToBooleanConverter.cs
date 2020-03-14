using System;
using System.Globalization;
using System.Windows.Data;
using Acolyte.Assertions;
using System.Linq;

namespace ThingAppraiser.DesktopApp.Domain.Converters
{
    internal class InverseAndBooleansToBooleanConverter : IMultiValueConverter
    {
        public InverseAndBooleansToBooleanConverter()
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
                return values.All(value => value is bool booleanValue && !booleanValue);
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException($"{nameof(ConvertBack)} is not implemented.");
        }

        #endregion
    }
}
