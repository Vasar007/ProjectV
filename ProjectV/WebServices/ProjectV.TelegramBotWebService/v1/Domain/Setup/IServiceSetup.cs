using System.Threading.Tasks;

namespace ProjectV.TelegramBotWebService.v1.Domain.Setup
{
    public interface IServiceSetup
    {
        Task<DisposableActionAsync> SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
