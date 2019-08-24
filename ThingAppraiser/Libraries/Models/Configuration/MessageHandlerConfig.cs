using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    [Serializable]
    public sealed class MessageHandlerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string MessageHandlerType { get; set; }

        [XmlAnyElement(Name = "MessageHandlerParameters")]
        public XElement[] MessageHandlerParameters { get; set; }


        public MessageHandlerConfig()
        {
        }
    }
}
