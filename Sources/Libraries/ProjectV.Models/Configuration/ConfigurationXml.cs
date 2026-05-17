using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    [XmlRoot(ElementName = nameof(ConfigurationXml), Namespace = "")]
    public sealed class ConfigurationXml
    {
        [XmlElement]
        public ShellConfig ShellConfig { get; set; } = default!;

        [XmlElement]
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType ServiceType { get; set; }


        public ConfigurationXml()
        {
        }
    }
}
