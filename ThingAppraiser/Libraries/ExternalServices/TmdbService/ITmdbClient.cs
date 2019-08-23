using System;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace ThingAppraiser.TmdbService
{
    public interface ITmdbClient
    {
        Task<SearchContainer<SearchMovie>> SearchMovieAsync(string query, int page = 0,
            bool includeAdult = false, int year = 0, CancellationToken cancellationToken = default);
    }
}
