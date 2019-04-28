using System;
using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IResultRepository : IRepository<CResultInfo, Guid>, ITagable, ITypeID
    {
        List<CThingIDWithRating> GetOrderedRatingsValue(Guid ratingID);
    }
}
