using System.Xml.Serialization;

namespace ThingAppraiser.Data.Configuration
{
    public enum ServiceType
    {
        [XmlEnum]
        Sequential,

        [XmlEnum]
        TplDataflow,

        [XmlEnum]
        Rx
    }
}
