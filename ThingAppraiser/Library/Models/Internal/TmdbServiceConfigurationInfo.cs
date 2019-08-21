using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ThingAppraiser.Models.Internal
{
    /// <summary>
    /// Represent configuration of the TMDb service.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of parameters! They must match the values received in 
    /// JSON.
    /// </remarks>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class TmdbServiceConfigurationInfo : IOptionalData
    {
        /// <summary>
        /// Provides base url to image server.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Provides secure url to image server.
        /// </summary>
        public string SecureBaseUrl { get; }

        /// <summary>
        /// Contains available backdrop sizes in string format.
        /// </summary>
        public IReadOnlyList<string> BackdropSizes { get; }

        /// <summary>
        /// Contains available poster sizes in string format.
        /// </summary>
        public IReadOnlyList<string> PosterSizes { get; }

        #region IOptionalData Implementation

        /// <summary>
        /// Represents kind of additional value. This property used only for JSON (de)serialization.
        /// </summary>
        public string Kind { get; } = nameof(TmdbServiceConfigurationInfo);

        #endregion


        /// <summary>
        /// Initializes configuration of the service.
        /// </summary>
        /// <param name="baseUrl">Base url to image server.</param>
        /// <param name="secureBaseUrl">Secure url to image server.</param>
        /// <param name="backdropSizes">Collection of backdrop sizes in string format.</param>
        /// <param name="posterSizes">Collection of poster sizes in string format.</param>
        [JsonConstructor]
        public TmdbServiceConfigurationInfo(string baseUrl, string secureBaseUrl,
            List<string> backdropSizes, List<string> posterSizes)
        {
            BaseUrl = baseUrl;
            SecureBaseUrl = secureBaseUrl;
            BackdropSizes = backdropSizes;
            PosterSizes = posterSizes;
        }
    }
}
