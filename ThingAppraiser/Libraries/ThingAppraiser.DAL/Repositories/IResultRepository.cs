using System;
using System.Collections.Generic;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IResultRepository : IRepository<ResultInfo, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
        IReadOnlyList<ThingIdWithRating> GetOrderedRatingsValue(Guid ratingId);
    }
}
