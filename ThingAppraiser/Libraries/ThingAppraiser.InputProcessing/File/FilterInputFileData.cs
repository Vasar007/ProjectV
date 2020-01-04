using FileHelpers;

namespace ThingAppraiser.IO.Input.File
{
    // TODO: refactor FieldOrderAttribute and allow to work with properties and immutable models.
    /// <summary>
    /// Represents record which could be used by FileHelper reader. This data class is used by
    /// filter reader.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
    public sealed class FilterInputFileData
    {
        /// <summary>
        /// Finds in header "Status" column and try to read its values.
        /// </summary>
        [FieldOrder(1), FieldTitle("Status")]
        public string status = default!;

        /// <summary>
        /// Finds in header "Thing Name" column and try to read its values.
        /// </summary>
        [FieldOrder(2), FieldTitle("Thing Name")]
        public string thingName = default!;


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public FilterInputFileData()
        {
        }
    }
}
