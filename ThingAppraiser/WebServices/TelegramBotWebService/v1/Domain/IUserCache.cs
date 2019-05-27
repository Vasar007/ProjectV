using ThingAppraiser.Data.Models;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IUserCache
    {
        bool TryAddUser(long id, RequestParams requestParams);

        bool TryGetValue(long id, out RequestParams requestParams);

        bool TryRemoveUser(long id);

        bool TryRemoveUser(long id, out RequestParams requestParams);
    }
}
