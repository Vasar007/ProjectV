using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class DatabaseManagerConfig
    {

        [XmlAnyElement(Name = nameof(DatabaseManagerParameters))]
        public XElement[] DatabaseManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = nameof(Repositories))]
        public XElement[] Repositories { get; set; } = default!;


        public DatabaseManagerConfig()
        {
        }
    }
}
