using System.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Mappers
{
    public sealed class ThingIdWithRatingMapper : IMapper<ThingIdWithRating>
    {
        public ThingIdWithRatingMapper()
        {
        }

        #region IMapper<ThingIdWithRating> Implementation

        public ThingIdWithRating ReadItem(IDataReader reader)
        {
            return new ThingIdWithRating(
                id:     (int)    reader["thing_id"],
                rating: (double) reader["rating_value"]
            );
        }

        #endregion
    }
}
