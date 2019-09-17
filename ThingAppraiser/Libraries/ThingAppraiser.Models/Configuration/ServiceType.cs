using System.Xml.Serialization;

namespace ThingAppraiser.Models.Configuration
{
    public enum ServiceType
    {
        [XmlEnum]
        Sequential,

        [XmlEnum]
        TplDataflow
    }
}
