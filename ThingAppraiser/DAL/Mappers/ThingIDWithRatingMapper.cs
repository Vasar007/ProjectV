using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class ThingIdWithRatingMapper : IMapper<ThingIdWithRating>
    {
        public ThingIdWithRatingMapper()
        {
        }

        #region IMapper<ThingIdWithRating> Implementation

        public ThingIdWithRating ReadItem(IDataReader reader)
        {
            var item = new ThingIdWithRating(
                id:     (int)    reader["thing_id"],
                rating: (double) reader["rating_value"]
            );
            return item;
        }

        #endregion
    }
}
