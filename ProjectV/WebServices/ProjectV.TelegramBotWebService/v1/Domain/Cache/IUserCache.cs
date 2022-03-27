using System.Diagnostics.CodeAnalysis;
using ProjectV.Models.WebService.Requests;

namespace ProjectV.TelegramBotWebService.v1.Domain.Cache
{
    public interface IUserCache
    {
        bool TryAddUser(long id, StartJobParamsRequest jobParams);

        bool TryGetUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams);

        bool TryRemoveUser(long id);

        bool TryRemoveUser(long id, [MaybeNullWhen(false)] out StartJobParamsRequest jobParams);
    }
}
