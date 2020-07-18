using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class MessageHandlerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string MessageHandlerType { get; set; } = default!;

        [XmlAnyElement(Name = "MessageHandlerParameters")]
        public XElement[] MessageHandlerParameters { get; set; } = default!;


        public MessageHandlerConfig()
        {
        }
    }
}
