using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    public enum ServiceType
    {
        [XmlEnum]
        Sequential,

        [XmlEnum]
        TplDataflow
    }
}
