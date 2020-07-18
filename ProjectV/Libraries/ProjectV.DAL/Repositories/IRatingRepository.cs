using System;
using ProjectV.Models.Internal;

namespace ProjectV.DAL.Repositories
{
    public interface IRatingRepository : IRepository<Rating, Guid>, IRepositoryBase, ITagable,
        ITypeId
    {
    }
}
