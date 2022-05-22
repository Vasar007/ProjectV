using System.Threading.Tasks;
using Acolyte.Common.Disposal;

namespace ProjectV.TelegramBotWebService.v1.Domain.Setup
{
    public interface IServiceSetup
    {
        Task<AsyncDisposableAction> SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
