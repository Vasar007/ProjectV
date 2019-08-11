using System.Threading.Tasks;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IServiceProxy
    {
        Task<ProcessingResponse> SendPostRequest(RequestParams requestParams);
    }
}
