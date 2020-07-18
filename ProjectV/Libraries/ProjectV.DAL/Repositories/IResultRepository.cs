using System;
using System.Collections.Generic;
using ProjectV.Models.Internal;

namespace ProjectV.DAL.Repositories
{
    public interface IResultRepository : IRepository<ResultInfo, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
        IReadOnlyList<ThingIdWithRating> GetOrderedRatingsValue(Guid ratingId);
    }
}
