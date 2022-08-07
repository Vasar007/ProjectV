using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectV.TelegramBotWebService.v1.Domain.Handlers
{
    public interface IBotHandler<TType> : IDisposable
    {
        Task ProcessAsync(TType type, CancellationToken cancellationToken);
    }
}
