using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class CrawlersManagerConfig
    {
        [XmlAttribute(DataType = "boolean")]
        public bool CrawlersOutputFlag { get; set; }

        [XmlAnyElement(Name = nameof(CrawlersManagerParameters))]
        public XElement[] CrawlersManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = nameof(Crawlers))]
        public XElement[] Crawlers { get; set; } = default!;


        public CrawlersManagerConfig()
        {
        }
    }
}
