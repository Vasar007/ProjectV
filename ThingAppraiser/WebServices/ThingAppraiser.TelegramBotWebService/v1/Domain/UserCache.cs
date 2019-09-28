using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public sealed class UserCache : IUserCache
    {
        private readonly ConcurrentDictionary<long, RequestParams> _cache =
            new ConcurrentDictionary<long, RequestParams>();


        public UserCache()
        {
        }

        #region IUserCache Implementation

        public bool TryAddUser(long id, RequestParams requestParams)
        {
            requestParams.ThrowIfNull(nameof(requestParams));

            return _cache.TryAdd(id, requestParams);
        }

        public bool TryGetUser(long id, [MaybeNullWhen(false)] out RequestParams? requestParams)
        {
            return _cache.TryGetValue(id, out requestParams);
        }


        public bool TryRemoveUser(long id)
        {
            return _cache.TryRemove(id, out RequestParams _);
        }

        public bool TryRemoveUser(long id, [MaybeNullWhen(false)] out RequestParams? requestParams)
        {
            return _cache.TryRemove(id, out requestParams);
        }
        #endregion
    }
}
