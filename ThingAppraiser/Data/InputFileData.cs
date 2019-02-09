using FileHelpers;

namespace ThingAppraiser.Data
{
    [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
    public class InputFileData
    {
        [FieldOrder(1), FieldTitle("Thing Name")]
        public string thingName = default(string); // Default assignement to remove warning.
    }
}
