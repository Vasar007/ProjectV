using System;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class RatingMapper : IMapper<Rating>
    {
        public RatingMapper()
        {
        }

        #region IMapper<Rating> Implementation

        public Rating ReadItem(IDataReader reader)
        {
            var item = new Rating(
                (Guid)   reader["rating_id"],
                (string) reader["rating_name"]
            );
            return item;
        }

        #endregion
    }
}
