using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProjectV.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class AppraisersManagerConfig
    {
        [XmlAttribute(DataType = "boolean")]
        public bool AppraisersOutputFlag { get; set; }

        [XmlAnyElement(Name = nameof(AppraisersManagerParameters))]
        public XElement[] AppraisersManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = nameof(Appraisers))]
        public XElement[] Appraisers { get; set; } = default!;


        public AppraisersManagerConfig()
        {
        }
    }
}
