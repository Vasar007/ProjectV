using System.Threading.Tasks;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public interface IProcessingResponseReceiverAsync
    {
        Task<ProcessingResponse> ReceiveProcessingResponseAsync(RequestData requestData);
    }
}
