using System;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace ThingAppraiser.Models.Configuration
{
    [Serializable]
    public sealed class AppraisersManagerConfig
    {
        [XmlAttribute(DataType = "boolean")]
        public bool AppraisersOutputFlag { get; set; }

        [XmlAnyElement(Name = "AppraisersManagerParameters")]
        public XElement[] AppraisersManagerParameters { get; set; }

        [XmlAnyElement(Name = "Appraisers")]
        public XElement[] Appraisers { get; set; }


        public AppraisersManagerConfig()
        {
        }
    }
}
