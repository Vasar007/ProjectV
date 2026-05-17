using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ProjectV.Models.Internal;
using ProjectV.TmdbService.Models;

namespace ProjectV.TmdbService
{
    public interface ITmdbClient : IDisposable
    {
        /// <summary>
        /// The maximum number of times a call to TMDb will be retried.
        /// </summary>
        /// <remarks>Default is 0</remarks>
        int MaxRetryCount { get; set; }

        /// <summary>
        /// Throw exceptions when TMDbs API returns certain errors, such as Not Found.
        /// </summary>
        bool ThrowApiExceptions { get; set; }

        bool HasConfig { get; }

        /// <summary>
        /// ISO 639-1 code. Example: en.
        /// </summary>
        string DefaultLanguage { get; set; }

        /// <summary>
        /// ISO 3166-1 code. Example: US.
        /// </summary>
        string DefaultCountry { get; set; }

        TmdbServiceConfigurationInfo Config { get; }

        /// <summary>
        /// Key to get access to TMDb service.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Gets or sets the Web Proxy to use during requests to TMDb API.
        /// </summary>
        /// <remarks>
        /// The Web Proxy is optional. If set, every request will be sent through it.
        /// Use the constructor for setting it.
        ///
        /// For convenience, this library also offers a <see cref="IWebProxy"/> implementation.
        /// Check <see cref="TMDbLib.Utilities.TMDbAPIProxy"/> for more information.
        /// </remarks>
        IWebProxy WebProxy { get; }


        Task<TmdbSearchContainer?> TrySearchMovieAsync(string query, int page = 0,
            bool includeAdult = false, int year = 0, string? region = null,
            int primaryReleaseYear = 0, CancellationToken cancellationToken = default);

        Task<TmdbServiceConfigurationInfo> GetConfigAsync();
    }
}
