using System.Collections.Generic;

namespace ThingAppraiser.Data.Models
{
    public class ProcessingResponse
    {
        public ResponseMetaData MetaData { get; set; }

        public List<List<RatingDataContainer>> RatingDataContainers { get; set; }


        public ProcessingResponse()
        {
        }
    }
}
