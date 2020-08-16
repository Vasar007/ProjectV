using System.Threading.Tasks;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IServiceSetup
    {
        Task<DisposableActionAsync> SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
