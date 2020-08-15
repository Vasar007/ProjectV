using ProjectV.Models.Data;

namespace ProjectV.DataAccessLayer.Repositories
{
    public interface IDataRepository : IRepository<BasicInfo, int>, IRepositoryBase, ITagable,
        ITypeId
    {
        T GetMinimum<T>(string columnName)
            where T : struct;

        T GetMaximum<T>(string columnName) 
            where T : struct;

        (T, T) GetMinMax<T>(string columnName) 
            where T : struct;
    }
}
