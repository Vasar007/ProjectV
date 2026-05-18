using System.Threading;
using System.Threading.Tasks;
using Acolyte.Common.Disposal;

namespace ProjectV.TelegramBotWebService.v1.Domain.Webhooks
{
    public interface IBotWebhook
    {
        Task<AsyncDisposableAction> SetWebhookAsync(CancellationToken cancellationToken);

        Task DeleteWebhookAsync(CancellationToken cancellationToken);
    }
}
