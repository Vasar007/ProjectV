using System;
using System.Threading.Tasks;
using ProjectV.Models.Data;

namespace ProjectV.OmdbService
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
