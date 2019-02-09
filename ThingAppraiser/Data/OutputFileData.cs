using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace ThingAppraiser.Data
{
    [DelimitedRecord(","), IgnoreEmptyLines(true)]
    public class OuputFileData
    {
        public class RatingValueConverter : ConverterBase
        {
            private const char _separator = '|';

            public override object StringToField(string from)
            {
                var values = from.Split(_separator).ToList();
                return values.ConvertAll(value => float.Parse(value));
            }

            public override string FieldToString(object fieldValue)
            {
                if (fieldValue is List<float> value)
                {

                    return string.Join(_separator + " ", value.ToArray());
                }
                else
                {
                    throw new ArgumentException("Object value is not a List<float>");
                }
            }
        }

        [FieldOrder(1), FieldTitle("Thing Name")]
        public string thingName = default(string); // Default assignement to remove warning.

        [FieldOrder(2), FieldTitle("Rating Value"), FieldConverter(typeof(RatingValueConverter))]
        public List<float> ratingValue = default(List<float>); // Default assignement to remove warning.
    }
}
