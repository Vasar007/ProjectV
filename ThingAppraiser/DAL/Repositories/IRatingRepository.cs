using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IRatingRepository : IRepository<CRating, Guid>, ITagable, ITypeID
    {
    }
}
