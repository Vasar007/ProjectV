using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IDataRepository : IRepository<BasicInfo, int>, IRepositoryBase, ITagable,
        ITypeId
    {
        T GetMinimum<T>(string columnName);

        T GetMaximum<T>(string columnName);

        (T, T) GetMinMax<T>(string columnName);
    }
}
