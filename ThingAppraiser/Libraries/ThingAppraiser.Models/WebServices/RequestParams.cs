using System.Collections.Generic;

namespace ThingAppraiser.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class RequestParams
    {
        public List<string> ThingNames { get; set; } = default!;

        public ConfigRequirements Requirements { get; set; } = default!;


        public RequestParams()
        {
        }
    }
}
