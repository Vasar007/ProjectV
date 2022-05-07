using System;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Processing
{
    public interface IProcessingResponseReceiverAsync : IDisposable
    {
        Task<Result<ProcessingResponse, ErrorResponse>> ReceiveProcessingResponseAsync(StartJobDataResponce jobData);
    }
}
