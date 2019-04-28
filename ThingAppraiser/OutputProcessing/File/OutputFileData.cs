using System;
using System.Collections.Generic;
using FileHelpers;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Represents record which could be used by FileHelper writer.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true)]
    public class COuputFileData
    {
        /// <summary>
        /// Name of appraised Thing.
        /// </summary>
        [FieldOrder(1), FieldTitle("Thing Name")]
        public String thingName = default(String); // Default assignment to remove warning.

        /// <summary>
        /// Rating values of all appraisers. Appraisers list user can specify in config.
        /// </summary>
        [FieldOrder(2), FieldTitle("Rating Value"), FieldConverter(typeof(CRatingValueConverter))]
        public List<Double> ratingValue = default(List<Double>); // Default assignment to remove warning.
    }
}
