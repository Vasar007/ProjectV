using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class DataBaseManagerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string ConnectionString { get; set; } = default!;

        [XmlAnyElement(Name = "DataBaseManagerParameters")]
        public XElement[] DataBaseManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = "Repositories")]
        public XElement[] Repositories { get; set; } = default!;


        public DataBaseManagerConfig()
        {
        }
    }
}
