using System.Threading.Tasks;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IServiceSetupAsync
    {
        Task<DisposableActionAsync> SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
