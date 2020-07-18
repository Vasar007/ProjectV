using System.Threading.Tasks;
using ProjectV.Models.WebService;

namespace ProjectV.CommunicationWebService.v1.Domain
{
    public interface IProcessingResponseReceiverAsync
    {
        Task<ProcessingResponse> ReceiveProcessingResponseAsync(RequestData requestData);
    }
}
