using System;
using System.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Mappers
{
    public sealed class RatingMapper : IMapper<Rating>
    {
        public RatingMapper()
        {
        }

        #region IMapper<Rating> Implementation

        public Rating ReadItem(IDataReader reader)
        {
            return new Rating(
                (Guid)   reader["rating_id"],
                (string) reader["rating_name"]
            );
        }

        #endregion
    }
}
