using System.Xml.Serialization;
using System.Xml.Linq;

namespace ThingAppraiser.Models.Configuration
{
    // TODO: make this DTO immutable.
    public sealed class AppraisersManagerConfig
    {
        [XmlAttribute(DataType = "boolean")]
        public bool AppraisersOutputFlag { get; set; }

        [XmlAnyElement(Name = "AppraisersManagerParameters")]
        public XElement[] AppraisersManagerParameters { get; set; } = default!;

        [XmlAnyElement(Name = "Appraisers")]
        public XElement[] Appraisers { get; set; } = default!;


        public AppraisersManagerConfig()
        {
        }
    }
}
