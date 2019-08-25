using System.Collections.Generic;
using ThingAppraiser.Models.Configuration;

namespace ThingAppraiser.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class RequestData
    {
        public List<string> ThingNames { get; set; } = default!;

        public ConfigurationXml ConfigurationXml { get; set; } = default!;


        public RequestData()
        {
        }
    }
}
