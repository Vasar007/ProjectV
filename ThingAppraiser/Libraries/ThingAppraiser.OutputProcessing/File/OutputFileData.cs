using System.Collections.Generic;
using FileHelpers;

namespace ThingAppraiser.IO.Output.File
{
    // TODO: refactor FieldOrderAttribute and allow to work with propertis and immutable models.
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
        public string thingName = default!;

        /// <summary>
        /// Rating values of all appraisers. Appraisers list user can specify in config.
        /// </summary>
        [FieldOrder(2), FieldTitle("Rating Value"), FieldConverter(typeof(RatingValueConverter))]
        public List<double> ratingValue = default!;


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public OuputFileData()
        {
        }
    }
}
