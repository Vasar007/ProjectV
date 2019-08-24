using System;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace ThingAppraiser.Models.Configuration
{
    [Serializable]
    public sealed class DataBaseManagerConfig
    {
        [XmlAttribute(DataType = "string")]
        public string ConnectionString { get; set; }

        [XmlAnyElement(Name = "DataBaseManagerParameters")]
        public XElement[] DataBaseManagerParameters { get; set; }

        [XmlAnyElement(Name = "Repositories")]
        public XElement[] Repositories { get; set; }


        public DataBaseManagerConfig()
        {
        }
    }
}
