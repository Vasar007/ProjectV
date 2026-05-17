using System.Threading.Tasks;
using ProjectV.Models.Basic;

namespace ProjectV.DataAccessLayer.Services.Basic
{
    public interface IDataInfoServiceBase<TId, TInfo>
        where TId : struct
        where TInfo : class, IEntity<TId>
    {
        Task<int> AddAsync(TInfo info);

        Task<TInfo?> FindByIdAsync(TId id);

        Task<TInfo> GetByIdAsync(TId id);

        Task<int> UpdateAsync(TInfo info);

        Task<int> DeleteAsync(TId id);
    }
}
