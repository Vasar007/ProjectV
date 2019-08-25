using System;
using System.Threading.Tasks;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IServiceSetupAsync
    {
        Task SetWebhookAsync();

        Task DeleteWebhookAsync();
    }
}
