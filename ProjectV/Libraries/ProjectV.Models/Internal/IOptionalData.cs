using JsonSubTypes;
using Newtonsoft.Json;

namespace ProjectV.Models.Internal
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
