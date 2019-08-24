using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    // TODO: make this DTO immutable.
    [Serializable]
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
