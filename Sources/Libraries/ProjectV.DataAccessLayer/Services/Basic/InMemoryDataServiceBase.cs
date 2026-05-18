using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Collections.Concurrent;
using ProjectV.Models.Basic;

namespace ProjectV.DataAccessLayer.Services.Basic
{
    public abstract class InMemoryDataServiceBase<TId, TInfo> : DataServiceBase<TId, TInfo>
        where TId : struct
        where TInfo : class, IEntity<TId>
    {
        protected readonly IConcurrentDictionary<TId, TInfo> _storage;

        internal event Func<TInfo, Task>? OnAddAsync;
        internal event Func<TInfo, Task>? OnUpdateAsync;
        internal event Func<TInfo, Task>? OnDeleteAsync;


        protected InMemoryDataServiceBase(
            IConcurrentDictionary<TId, TInfo> storage)
        {
            _storage = storage.ThrowIfNull(nameof(storage));
        }

        public virtual async Task<int> AddAsync(TInfo info)
        {
            info.ThrowIfNull(nameof(info));

            if (_storage.TryAdd(info.Id, info))
            {
                if (OnAddAsync is not null)
                {
                    await OnAddAsync.Invoke(info);
                }

                return 1;
            }

            return 0;
        }

        public override Task<TInfo?> FindByIdAsync(TId id)
        {
            if (_storage.TryGetValue(id, out TInfo? info))
            {
                return Task.FromResult<TInfo?>(info);
            }

            return Task.FromResult<TInfo?>(null);
        }

        public virtual async Task<int> UpdateAsync(TInfo info)
        {
            info.ThrowIfNull(nameof(info));

            TInfo? foundInfo = await FindByIdAsync(info.Id);
            if (foundInfo is not null)
            {
                if (_storage.TryUpdate(info.Id, info, foundInfo))
                {
                    if (OnUpdateAsync is not null)
                    {
                        await OnUpdateAsync.Invoke(info);
                    }

                    return 1;
                }
            }

            return 0;
        }

        public virtual async Task<int> DeleteAsync(TId id)
        {
            if (_storage.TryRemove(id, out TInfo? info))
            {
                if (OnDeleteAsync is not null)
                {
                    await OnDeleteAsync.Invoke(info);
                }

                return 1;
            }

            return 0;
        }
    }
}
