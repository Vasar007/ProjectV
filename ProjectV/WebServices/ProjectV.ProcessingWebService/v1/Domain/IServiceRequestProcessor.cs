using System.Threading.Tasks;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public interface IServiceRequestProcessor
    {
        Task<ProcessingResponse> ProcessRequest(StartJobDataResponce jobData);
    }
}
