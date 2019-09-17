using System;
using System.Threading.Tasks;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.OmdbService
{
    public interface IOmdbClient : IDisposable
    {
        /// <summary>
        /// Key to get access to OMDb service.
        /// </summary>
        string ApiKey { get; }

        bool UseRottenTomatoesRatings { get; }


        Task<OmdbMovieInfo?> TryGetItemByTitleAsync(string title, bool fullPlot = false);
    }
}
