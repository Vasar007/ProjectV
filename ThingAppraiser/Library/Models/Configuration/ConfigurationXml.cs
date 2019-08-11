using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ThingAppraiser.Models.Configuration
{
    [Serializable]
    [XmlRoot(ElementName = "Configuration", Namespace = "")]
    public class ConfigurationXml
    {
        [XmlElement]
        public ShellConfig ShellConfig { get; set; }

        [XmlElement]
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType ServiceType { get; set; }


        public ConfigurationXml()
        {
        }
    }
}
