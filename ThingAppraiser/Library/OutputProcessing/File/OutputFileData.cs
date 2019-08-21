using System.Collections.Generic;
using FileHelpers;

namespace ThingAppraiser.IO.Output.File
{
    /// <summary>
    /// Represents record which could be used by FileHelper writer.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true)]
    public sealed class OuputFileData
    {
        /// <summary>
        /// Name of appraised Thing.
        /// </summary>
        [FieldOrder(1), FieldTitle("Thing Name")]
        public string thingName = default; // Default assignment to remove warning.

        /// <summary>
        /// Rating values of all appraisers. Appraisers list user can specify in config.
        /// </summary>
        [FieldOrder(2), FieldTitle("Rating Value"), FieldConverter(typeof(RatingValueConverter))]
        public List<double> ratingValue = default; // Default assignment to remove warning.


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public OuputFileData()
        {
        }
    }
}
