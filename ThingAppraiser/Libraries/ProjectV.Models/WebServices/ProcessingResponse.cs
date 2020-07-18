using System.Collections.Generic;
using ProjectV.Models.Internal;

namespace ProjectV.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class ProcessingResponse
    {
        public ResponseMetadata Metadata { get; set; } = default!;

        public IReadOnlyList<IReadOnlyList<RatingDataContainer>> RatingDataContainers { get; set; }
            = default!;


        public ProcessingResponse()
        {
        }
    }
}
