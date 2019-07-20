using System.Collections.Generic;

namespace ThingAppraiser.Data.Models
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
