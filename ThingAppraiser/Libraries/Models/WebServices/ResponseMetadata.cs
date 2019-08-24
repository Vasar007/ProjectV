using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Models.WebService
{
    // TODO: make this DTO immutable.
    public sealed class ResponseMetadata
    {
        public int CommonResultsNumber { get; set; }

        public int CommonResultCollectionsNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceStatus ResultStatus { get; set; }

        public IReadOnlyDictionary<string, IOptionalData> OptionalData { get; set; } = default!;


        public ResponseMetadata()
        {
        }
    }
}