using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IUpdateService : IDisposable
    {
        Task ProcessUpdateRequestAsync(Update update, CancellationToken cancellationToken = default);
        Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken = default);
    }
}
