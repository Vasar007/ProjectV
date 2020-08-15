using System;
using System.Collections.Generic;
using ProjectV.Models.Internal;

namespace ProjectV.DataAccessLayer.Repositories
{
    public interface IResultRepository : IRepository<ResultInfo, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
        IReadOnlyList<ThingIdWithRating> GetOrderedRatingsValue(Guid ratingId);
    }
}
