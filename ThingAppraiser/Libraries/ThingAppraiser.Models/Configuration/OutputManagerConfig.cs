using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class OutputManagerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string DefaultOutStorageName { get; set; } = default!;

        [XmlAnyElement(Name = "OutputManagerParameters")]
        public XElement[] OutputManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = "Outputters")]
        public XElement[] Outputters { get; set; } = default!;


        public OutputManagerConfig()
        {
        }
    }
}
