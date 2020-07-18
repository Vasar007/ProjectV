using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class DataBaseManagerConfig
    {

        [XmlAnyElement(Name = "DataBaseManagerParameters")]
        public XElement[] DataBaseManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = "Repositories")]
        public XElement[] Repositories { get; set; } = default!;


        public DataBaseManagerConfig()
        {
        }
    }
}
