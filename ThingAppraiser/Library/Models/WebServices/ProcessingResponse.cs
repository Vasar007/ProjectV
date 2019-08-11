using System.Collections.Generic;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Models.WebService
{
    public class ProcessingResponse
    {
        public ResponseMetadata Metadata { get; set; }

        public List<List<RatingDataContainer>> RatingDataContainers { get; set; }


        public ProcessingResponse()
        {
        }
    }
}
