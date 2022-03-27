using System.Threading.Tasks;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Processing
{
    public interface IProcessingResponseReceiverAsync
    {
        Task<ProcessingResponse> ReceiveProcessingResponseAsync(StartJobDataResponce jobData);
    }
}
