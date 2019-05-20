using System.Collections.Generic;

namespace ThingAppraiser.Data.Models
{
    public class ConfigRequirements
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
