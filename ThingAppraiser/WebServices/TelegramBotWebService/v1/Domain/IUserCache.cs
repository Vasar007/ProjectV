using System.Diagnostics.CodeAnalysis;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IUserCache
    {
        bool TryAddUser(long id, RequestParams requestParams);

        bool TryGetUser(long id, [MaybeNullWhen(false)] out RequestParams? requestParams);

        bool TryRemoveUser(long id);

        bool TryRemoveUser(long id, [MaybeNullWhen(false)] out RequestParams? requestParams);
    }
}
