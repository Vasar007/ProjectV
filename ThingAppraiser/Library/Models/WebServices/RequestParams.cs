using System.Collections.Generic;

namespace ThingAppraiser.Models.WebService
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