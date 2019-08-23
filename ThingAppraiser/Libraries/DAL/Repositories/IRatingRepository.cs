using System;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IRatingRepository : IRepository<Rating, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
    }
}
