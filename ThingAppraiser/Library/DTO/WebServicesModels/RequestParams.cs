using System.Collections.Generic;

namespace ThingAppraiser.Data.Models
{
    public class RequestParams
    {
        public List<string> ThingNames { get; set; }

        public ConfigRequirements Requirements { get; set; }


        public RequestParams()
        {
        }
    }
}