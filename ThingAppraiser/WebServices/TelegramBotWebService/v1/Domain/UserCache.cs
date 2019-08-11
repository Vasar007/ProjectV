using System.Collections.Concurrent;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public class UserCache : IUserCache
    {
        private readonly ConcurrentDictionary<long, RequestParams> _cache;


        public UserCache()
        {
            _cache = new ConcurrentDictionary<long, RequestParams>();
        }

        #region IUserCache Implementation

        public bool TryAddUser(long id, RequestParams requestParams)
        {
            return _cache.TryAdd(id, requestParams);
        }

        public bool TryRemoveUser(long id)
        {
            return _cache.TryRemove(id, out RequestParams _);
        }

        public bool TryRemoveUser(long id, out RequestParams requestParams)
        {
            return _cache.TryRemove(id, out requestParams);
        }

        public bool TryGetValue(long id, out RequestParams requestParams)
        {
            return _cache.TryGetValue(id, out requestParams);
        }

        #endregion
    }
}
