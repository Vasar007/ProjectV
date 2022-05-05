using System.Collections.Generic;
using ProjectV.Models.Internal;

namespace ProjectV.Models.WebServices.Responses
{
    // TODO: make this DTO immutable.
    public sealed class ProcessingResponse
    {
        public ProcessingResponseMetadata Metadata { get; set; } = default!;

        public IReadOnlyList<IReadOnlyList<RatingDataContainer>> RatingDataContainers { get; set; }
            = default!;


        public ProcessingResponse()
        {
        }
    }
}
