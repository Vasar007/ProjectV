using System.Collections.Generic;
using ProjectV.Models.Configuration;

namespace ProjectV.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class RequestData
    {
        public IReadOnlyList<string> ThingNames { get; set; } = default!;

        public ConfigurationXml ConfigurationXml { get; set; } = default!;


        public RequestData()
        {
        }
    }
}
