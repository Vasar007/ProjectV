using System;
using System.Collections.Generic;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.TmdbService.Models
{
    public sealed class TmdbSearchContainer
    {
        public IReadOnlyList<TmdbMovieInfo> SearchResults { get; }

        public TmdbSearchContainer(IReadOnlyList<TmdbMovieInfo> searchResults)
        {
            SearchResults = searchResults.ThrowIfNull(nameof(searchResults));
        }
    }
}
