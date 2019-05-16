using JsonSubTypes;
using Newtonsoft.Json;
using ThingAppraiser.Data.Crawlers;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Represents optional data which used in meta data dictionary.
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "Kind")]
    [JsonSubtypes.KnownSubType(typeof(TmdbServiceConfigurationInfo),
                               nameof(TmdbServiceConfigurationInfo))]
    public interface IOptionalData
    {
        /// <summary>
        /// Field which used by JSON converter to know type of the object.
        /// </summary>
        string Kind { get; }
    }
}
