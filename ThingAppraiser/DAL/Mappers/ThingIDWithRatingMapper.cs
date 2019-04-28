using System;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class CThingIDWithRatingMapper : IMapper<CThingIDWithRating>
    {
        public CThingIDWithRatingMapper()
        {
        }

        #region IMapper<CBasicInfo> Implementation

        public CThingIDWithRating ReadItem(IDataReader reader)
        {
            var item = new CThingIDWithRating(
                id:     (Int32)  reader["thing_id"],
                rating: (Double) reader["rating_value"]
            );
            return item;
        }

        #endregion
    }
}
