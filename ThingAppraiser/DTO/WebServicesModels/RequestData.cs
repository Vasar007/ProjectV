using System.Collections.Generic;
using ThingAppraiser.Data.Configuration;

namespace ThingAppraiser.Data.Models
{
    public class RequestData
    {
        public List<string> ThingNames { get; set; }

        public ConfigurationXml ConfigurationXml { get; set; }


        public RequestData()
        {
        }
    }
}
