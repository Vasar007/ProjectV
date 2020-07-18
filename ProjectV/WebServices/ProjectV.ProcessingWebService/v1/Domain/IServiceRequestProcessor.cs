using System.Threading.Tasks;
using ProjectV.Models.WebService;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public interface IServiceRequestProcessor
    {
        Task<ProcessingResponse> ProcessRequest(RequestData requestData);
    }
}
