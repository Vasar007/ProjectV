using System.Collections.Generic;

namespace ThingAppraiser.Models.WebService
{
    public sealed class ConfigRequirements
    {
        public List<string> Input { get; set; }

        public List<string> Services { get; set; }

        public List<string> Appraisals { get; set; }

        public List<string> Output { get; set; }


        public ConfigRequirements()
        {
        }
    }
}
