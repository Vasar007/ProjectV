using System.Threading.Tasks;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IServiceProxy
    {
        Task<ProcessingResponse> SendPostRequest(RequestParams requestParams);
    }
}
