using System.Threading;
using System.Threading.Tasks;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling
{
    public interface IBotPolling
    {
        Task ResiveUpdatesAsync(CancellationToken cancellationToken);
    }
}
