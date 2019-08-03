using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Data.Configuration
{
    [Serializable]
    public class MessageHandlerConfig
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
