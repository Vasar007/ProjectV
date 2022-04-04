using System.Threading.Tasks;
using Acolyte.Exceptions;
using ProjectV.Models.Basic;

namespace ProjectV.DataAccessLayer.Services.Basic
{
    public abstract class DataServiceBase<TId, TInfo>
        where TId : struct
        where TInfo : class, IEntity<TId>
    {
        protected DataServiceBase()
        {
        }

        public abstract Task<TInfo?> FindByIdAsync(TId jobId);

        public virtual async Task<TInfo> GetByIdAsync(TId id)
        {
            TInfo? info = await FindByIdAsync(id);
            if (info is null)
            {
                throw new NotFoundException($"Failed to found info with ID '{id}'.");
            }

            return info;
        }
    }
}
