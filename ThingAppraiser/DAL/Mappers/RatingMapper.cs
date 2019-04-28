using System;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class CRatingMapper : IMapper<CRating>
    {
        public CRatingMapper()
        {
        }

        #region IMapper<CRating> Implementation

        public CRating ReadItem(IDataReader reader)
        {
            var item = new CRating(
                (Guid)   reader["rating_id"],
                (String) reader["rating_name"]
            );
            return item;
        }

        #endregion
    }
}
