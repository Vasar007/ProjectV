using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ThingAppraiser.Data.Models
{
    public class ResponseMetaData
    {
        public int CommonResultsNumber { get; set; }

        public int CommonResultCollectionsNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceStatus ResultStatus { get; set; }

        public IReadOnlyDictionary<string, IOptionalData> OptionalData { get; set; }


        public ResponseMetaData()
        {
        }
    }
}