using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Data.Configuration
{
    public class MessageHandlerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string MessageHandlerType { get; set; }

        [XmlAnyElement]
        public XElement[] MessageHandlerParameters { get; set; }


        public MessageHandlerConfig()
        {
        }
    }
}
