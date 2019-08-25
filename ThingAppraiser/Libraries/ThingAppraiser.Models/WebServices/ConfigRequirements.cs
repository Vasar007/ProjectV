using System.Collections.Generic;

namespace ThingAppraiser.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class ConfigRequirements
    {
        public List<string> Input { get; set; } = default!;

        public List<string> Services { get; set; } = default!;

        public List<string> Appraisals { get; set; } = default!;

        public List<string> Output { get; set; } = default!;


        public ConfigRequirements()
        {
        }
    }
}
