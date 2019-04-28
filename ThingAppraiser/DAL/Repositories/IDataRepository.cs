using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IDataRepository : IRepository<CBasicInfo, Int32>, ITagable, ITypeID
    {
        T GetMinimum<T>(String columnName);

        T GetMaximum<T>(String columnName);

        (T, T) GetMinMax<T>(String columnName);
    }
}
