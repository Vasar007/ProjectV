using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DAL.Mappers
{
    /// <remarks>
    /// It's not an error or typo to inherit from <see cref="IMapper{BasicInfo}" /> rather than
    /// <see cref="IMapper{TmdbMovieInfo}" />  because we must correspond
    /// <see cref="Repositories.IRepository" /> interface.
    /// </remarks>
    public sealed class TmdbMovieMapper : IMapper<BasicInfo>
    {
        public TmdbMovieMapper()
        {
        }

        #region IMapper<BasicInfo> Implementation

        public BasicInfo ReadItem(IDataReader reader)
        {
            var genreIdsAsString = (string) reader["genre_ids"];
            List<int> genreIds = genreIdsAsString.Split(',').Select(int.Parse).ToList();

            return new TmdbMovieInfo(
                thingId:     (int)      reader["thing_id"],
                title:       (string)   reader["title"],
                voteCount:   (int)      reader["vote_count"],
                voteAverage: (double)   reader["vote_average"],
                overview:    (string)   reader["overview"],
                releaseDate: (DateTime) reader["release_date"],
                popularity:  (double)   reader["popularity"],
                adult:       (bool)     reader["adult"],
                genreIds:               genreIds,
                posterPath:  (string)   reader["poster_path"]
            );
        }

        #endregion
    }
}
