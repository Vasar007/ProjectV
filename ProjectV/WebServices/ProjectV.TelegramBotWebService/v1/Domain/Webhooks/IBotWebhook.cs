using System.Threading.Tasks;
using Acolyte.Common.Disposal;

namespace ProjectV.TelegramBotWebService.v1.Domain.Webhooks
{
    public interface IBotWebhook
    {
        Task<AsyncDisposableAction> SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
