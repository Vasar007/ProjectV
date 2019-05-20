using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IRatingRepository : IRepository<Rating, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
    }
}
