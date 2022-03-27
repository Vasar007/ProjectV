using System.Collections.Generic;
using ProjectV.Models.Configuration;

namespace ProjectV.Models.WebService.Responses
{
    // TODO: make this DTO immutable.
    public sealed class StartJobDataResponce
    {
        public IReadOnlyList<string> ThingNames { get; set; } = default!;

        public ConfigurationXml ConfigurationXml { get; set; } = default!;


        public StartJobDataResponce()
        {
        }
    }
}
