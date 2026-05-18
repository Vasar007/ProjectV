using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProjectV.Models.Internal;

namespace ProjectV.Models.WebServices.Responses
{
    // TODO: make this DTO immutable.
    public sealed class ProcessingResponseMetadata
    {
        public int CommonResultsNumber { get; set; }

        public int CommonResultCollectionsNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceStatus ResultStatus { get; set; }

        public IReadOnlyDictionary<string, IOptionalData> OptionalData { get; set; } = default!;


        public ProcessingResponseMetadata()
        {
        }
    }
}
