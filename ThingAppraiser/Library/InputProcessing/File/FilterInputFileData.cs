using FileHelpers;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Represents record which could be used by FileHelper reader. This data class is used by
    /// filter reader.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
    public class FilterInputFileData
    {
        /// <summary>
        /// Finds in header "Status" column and try to read its values.
        /// </summary>
        [FieldOrder(1), FieldTitle("Status")]
        public string status = default; // Default assignment to remove warning.

        /// <summary>
        /// Finds in header "Thing Name" column and try to read its values.
        /// </summary>
        [FieldOrder(2), FieldTitle("Thing Name")]
        public string thingName = default; // Default assignment to remove warning.


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public FilterInputFileData()
        {
        }
    }
}
