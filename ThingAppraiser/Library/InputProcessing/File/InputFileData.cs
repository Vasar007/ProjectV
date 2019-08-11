using FileHelpers;

namespace ThingAppraiser.IO.Input.File
{
    /// <summary>
    /// Represents record which could be used by FileHelper reader.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
    public class InputFileData
    {
        /// <summary>
        /// Finds in header "Thing Name" column and try to read its values.
        /// </summary>
        [FieldOrder(1), FieldTitle("Thing Name")]
        public string thingName = default; // Default assignment to remove warning.


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public InputFileData()
        {
        }
    }
}
