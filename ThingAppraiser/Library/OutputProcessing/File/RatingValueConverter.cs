using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace ThingAppraiser.IO.Output.File
{
    /// <summary>
    /// Class which is used to convert list to string for FileHelpers library.
    /// </summary>
    public sealed class RatingValueConverter : ConverterBase
    {
        private static readonly char _separator = '|';


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public RatingValueConverter()
        {
        }

        #region ConverterBase Overridden Methods

        /// <inheritdoc />
        public override object StringToField(string from)
        {
            List<string> values = from.Split(_separator).ToList();
            return values.ConvertAll(double.Parse);
        }

        /// <inheritdoc />
        public override string FieldToString(object fieldValue)
        {
            if (fieldValue is List<double> value)
            {
                return string.Join(_separator + " ", value);
            }

            throw new ArgumentException("Object value is not a List<float>");
        }

        #endregion
    }
}
