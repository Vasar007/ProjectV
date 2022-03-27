using System.Threading.Tasks;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.TelegramBotWebService.v1.Domain.Proxy
{
    public interface IServiceProxy
    {
        Task<ProcessingResponse?> SendPostRequest(StartJobParamsRequest jobParams);
    }
}
