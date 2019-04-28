using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    /// <remarks>
    /// It's not an error or typo to inherit from <see cref="IMapper{CBasicInfo}" /> rather than
    /// <see cref="IMapper{CMovieTMDBInfo}" />  because we must correspond
    /// <see cref="Repositories.IRepository" /> interface.
    /// </remarks>
    public class CMovieTMDBMapper : IMapper<CBasicInfo>
    {
        public CMovieTMDBMapper()
        {
        }

        #region IMapper<CBasicInfo> Implementation

        public CBasicInfo ReadItem(IDataReader reader)
        {
            var genreIDsAsString = (String) reader["genre_ids"];
            List<Int32> genreIDs = genreIDsAsString.Split(',').Select(Int32.Parse).ToList();

            var item = new CMovieTMDBInfo(
                id:           (Int32) reader["thing_id"],
                title:        (String) reader["title"],
                vote_count:   (Int32) reader["vote_count"],
                vote_average: (Double) reader["vote_average"],
                overview:     (String) reader["overview"],
                release_date: (DateTime) reader["release_date"],
                popularity:   (Double) reader["popularity"],
                adult:        (Boolean) reader["adult"],
                genre_ids:    genreIDs,
                poster_path:  (String) reader["poster_path"]
            );
            return item;
        }

        #endregion
    }
}
