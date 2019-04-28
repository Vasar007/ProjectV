using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Represent configuration of the TMDB service.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of parameters! They must match the values received in 
    /// JSON.
    /// </remarks>
    [Serializable]
    public class CServiceConfigurationInfoTMDB
    {
        /// <summary>
        /// Provides base url to image server.
        /// </summary>
        public String BaseUrl { get; }

        /// <summary>
        /// Provides secure url to image server.
        /// </summary>
        public String SecureBaseUrl { get; }

        /// <summary>
        /// Contains available backdrop sizes in string format.
        /// </summary>
        public List<String> BackdropSizes { get; }

        /// <summary>
        /// Contains available poster sizes in string format.
        /// </summary>
        public List<String> PosterSizes { get; }


        /// <summary>
        /// Initializes configuration of the service.
        /// </summary>
        /// <param name="base_Url">Base url to image server.</param>
        /// <param name="secure_Base_Url">Secure url to image server.</param>
        /// <param name="backdrop_Sizes">Collection of backdrop sizes in string format.</param>
        /// <param name="poster_Sizes">Collection of poster sizes in string format.</param>
        [JsonConstructor]
        public CServiceConfigurationInfoTMDB(String base_Url, String secure_Base_Url,
            List<String> backdrop_Sizes, List<String> poster_Sizes)
        {
            BaseUrl = base_Url;
            SecureBaseUrl = secure_Base_Url;
            BackdropSizes = backdrop_Sizes;
            PosterSizes = poster_Sizes;
        }
    }
}
