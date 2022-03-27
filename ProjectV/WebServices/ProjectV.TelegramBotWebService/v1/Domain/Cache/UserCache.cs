using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Acolyte.Assertions;
using ProjectV.Models.WebService.Requests;

namespace ProjectV.TelegramBotWebService.v1.Domain.Cache
{
    public sealed class UserCache : IUserCache
    {
        private readonly ConcurrentDictionary<long, StartJobParamsRequest> _cache = new();


        public UserCache()
        {
        }

        #region IUserCache Implementation

        public bool TryAddUser(long id, StartJobParamsRequest requestParams)
        {
            requestParams.ThrowIfNull(nameof(requestParams));

            return _cache.TryAdd(id, requestParams);
        }

        public bool TryGetUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams)
        {
            return _cache.TryGetValue(id, out jobParams);
        }


        public bool TryRemoveUser(long id)
        {
            return _cache.TryRemove(id, out StartJobParamsRequest _);
        }

        public bool TryRemoveUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams)
        {
            return _cache.TryRemove(id, out jobParams);
        }

        #endregion
    }
}
