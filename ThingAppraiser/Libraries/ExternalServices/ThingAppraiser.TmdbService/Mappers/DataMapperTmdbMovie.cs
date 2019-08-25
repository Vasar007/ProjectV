using System;
using ThingAppraiser.Models.Data;
using TMDbLib.Objects.Search;

namespace ThingAppraiser.TmdbService.Mappers
{
    public sealed class DataMapperTmdbMovie : IDataMapper<SearchMovie, TmdbMovieInfo>
    {
        public DataMapperTmdbMovie()
        {
        }

        #region IDataMapper<SearchMovie, TmdbMovieInfo> Implementation

        public TmdbMovieInfo Transform(SearchMovie dataObject)
        {
            return new TmdbMovieInfo(
                thingId:     dataObject.Id,
                title:       dataObject.Title,
                voteCount:   dataObject.VoteCount,
                voteAverage: dataObject.VoteAverage,
                overview:    dataObject.Overview,
                releaseDate: dataObject.ReleaseDate ?? new DateTime(),
                popularity:  dataObject.Popularity,
                adult:       dataObject.Adult,
                genreIds:    dataObject.GenreIds,
                posterPath:  dataObject.PosterPath
            );
        }

        #endregion
    }
}
