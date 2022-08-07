using Acolyte.Collections.Concurrent;
using ProjectV.Models.Basic;

namespace ProjectV.DataAccessLayer.Services.Basic
{
    public class InMemoryDataService<TId, TInfo> : InMemoryDataServiceBase<TId, TInfo>
        where TId : struct
        where TInfo : class, IEntity<TId>
    {
        public InMemoryDataService()
            : base(new ConcurrentDictionaryWrapper<TId, TInfo>())
        {
        }
    }
}
