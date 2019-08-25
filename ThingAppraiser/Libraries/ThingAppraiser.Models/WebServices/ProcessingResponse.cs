using System.Collections.Generic;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class ProcessingResponse
    {
        public ResponseMetadata Metadata { get; set; } = default!;

        public List<List<RatingDataContainer>> RatingDataContainers { get; set; } = default!;


        public ProcessingResponse()
        {
        }
    }
}
