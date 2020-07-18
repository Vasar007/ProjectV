using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class InputManagerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string DefaultInStorageName { get; set; } = default!;

        [XmlAnyElement(Name = "InputManagerParameters")]
        public XElement[] InputManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = "Inputters")]
        public XElement[] Inputters { get; set; } = default!;


        public InputManagerConfig()
        {
        }
    }
}
