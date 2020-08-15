using System;
using ProjectV.Models.Internal;

namespace ProjectV.DataAccessLayer.Repositories
{
    public interface IRatingRepository : IRepository<Rating, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
    }
}
