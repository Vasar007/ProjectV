using FileHelpers;

namespace ProjectV.IO.Input.File
{
    // TODO: refactor FieldOrderAttribute and allow to work with properties and immutable models.
    /// <summary>
    /// Represents record which could be used by FileHelper reader.
    /// </summary>
    /// <remarks>
    /// FileHelper doesn't support properties. That's why this data class contains public field.
    /// </remarks>
    [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
    public sealed class InputFileData
    {
        /// <summary>
        /// Finds in header "Thing Name" column and try to read its values.
        /// </summary>
        [FieldOrder(1), FieldTitle("Thing Name")]
        public string thingName = default!;


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public InputFileData()
        {
        }
    }
}
