using System.Threading.Tasks;
using ProjectV.Models.WebService;

namespace ProjectV.TelegramBotWebService.v1.Domain.Proxy
{
    public interface IServiceProxy
    {
        Task<ProcessingResponse?> SendPostRequest(RequestParams requestParams);
    }
}
