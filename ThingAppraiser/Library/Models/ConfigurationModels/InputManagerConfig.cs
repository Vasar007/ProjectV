using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.Data.Configuration
{
    [Serializable]
    public class InputManagerConfig
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
