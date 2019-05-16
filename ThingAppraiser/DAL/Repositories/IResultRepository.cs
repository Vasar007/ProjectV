using System;
using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IResultRepository : IRepository<ResultInfo, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
        List<ThingIdWithRating> GetOrderedRatingsValue(Guid ratingId);
    }
}
