using System.Collections.Generic;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.TmdbService.Models
{
    public sealed class TmdbSearchContainer
    {
        public int Page { get; }

        public IReadOnlyList<TmdbMovieInfo> Results { get; }

        public int TotalPages { get; }

        public int TotalResults { get; }


        public TmdbSearchContainer(int page, IReadOnlyList<TmdbMovieInfo> results, int totalPages,
            int totalResults)
        {
            Page = page;
            Results = results.ThrowIfNull(nameof(results));
            TotalPages = totalPages;
            TotalResults = totalResults;
        }
    }
}
