using System.Threading.Tasks;
using ProjectV.Models.WebService;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IServiceProxy
    {
        Task<ProcessingResponse?> SendPostRequest(RequestParams requestParams);
    }
}
