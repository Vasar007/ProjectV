using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    [Serializable]
    public sealed class InputManagerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string DefaultInStorageName { get; set; }

        [XmlAnyElement(Name = "InputManagerParameters")]
        public XElement[] InputManagerParameters { get; set; }

        [XmlAnyElement(Name = "Inputters")]
        public XElement[] Inputters { get; set; }


        public InputManagerConfig()
        {
        }
    }
}
