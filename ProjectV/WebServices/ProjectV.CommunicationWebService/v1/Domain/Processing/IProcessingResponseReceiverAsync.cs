using System.Threading.Tasks;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Processing
{
    public interface IProcessingResponseReceiverAsync
    {
        Task<ProcessingResponse> ReceiveProcessingResponseAsync(StartJobDataResponce jobData);
    }
}
