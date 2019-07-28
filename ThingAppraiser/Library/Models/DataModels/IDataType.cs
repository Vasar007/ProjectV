using JsonSubTypes;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Represents optional data which used to convert data handlers.
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "Kind")]
    [JsonSubtypes.KnownSubType(typeof(TmdbMovieInfo), nameof(TmdbMovieInfo))]
    [JsonSubtypes.KnownSubType(typeof(OmdbMovieInfo), nameof(OmdbMovieInfo))]
    [JsonSubtypes.KnownSubType(typeof(SteamGameInfo), nameof(SteamGameInfo))]
    public interface IDataType
    {
        /// <summary>
        /// Field which used by JSON converter to know type of the object.
        /// </summary>
        string Kind { get; }
    }
}
