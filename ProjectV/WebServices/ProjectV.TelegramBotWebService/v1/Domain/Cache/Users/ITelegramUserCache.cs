using System.Diagnostics.CodeAnalysis;
using ProjectV.Models.WebServices.Requests;

namespace ProjectV.TelegramBotWebService.v1.Domain.Cache.Users
{
    public interface ITelegramUserCache
    {
        bool TryAddUser(long id, StartJobParamsRequest jobParams);

        bool TryGetUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams);

        bool TryRemoveUser(long id);

        bool TryRemoveUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams);
    }
}
