using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which is used to convert list to string for FileHelpers library.
    /// </summary>
    public class CRatingValueConverter : ConverterBase
    {
        private static readonly Char s_separator = '|';


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CRatingValueConverter()
        {
        }

        #region ConverterBase Overridden Methods

        /// <inheritdoc />
        public override Object StringToField(String from)
        {
            List<String> values = from.Split(s_separator).ToList();
            return values.ConvertAll(Double.Parse);
        }

        /// <inheritdoc />
        public override String FieldToString(Object fieldValue)
        {
            if (fieldValue is List<Double> value)
            {
                return String.Join(s_separator + " ", value);
            }

            throw new ArgumentException("Object value is not a List<float>");
        }

        #endregion
    }
}
