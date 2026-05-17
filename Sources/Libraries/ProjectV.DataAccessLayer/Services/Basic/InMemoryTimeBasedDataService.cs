using System;
using Acolyte.Collections.Concurrent;
using Acolyte.Common;
using ProjectV.Models.Basic;

namespace ProjectV.DataAccessLayer.Services.Basic
{
    public class InMemoryTimeBasedDataService<TId, TInfo> : InMemoryDataServiceBase<TId, TInfo>
        where TId : struct
        where TInfo : class, IEntity<TId>, IHaveCreationTime
    {
        public InMemoryTimeBasedDataService(
            TimeSpan lifeTime)
            : base(new TimeBasedConcurrentDictionary<TId, TInfo>(lifeTime))
        {
        }
    }
}
