using System.Collections.Generic;

namespace ProjectV.Models.WebService.Requests
{
    // TODO: make this DTO immutable.
    public sealed class StartJobParamsRequest
    {
        public IReadOnlyList<string> ThingNames { get; set; } = default!;

        public ConfigRequirements Requirements { get; set; } = default!;


        public StartJobParamsRequest()
        {
        }
    }
}
