using System.Threading.Tasks;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.ProcessingWebService.v1.Domain
{
    public interface IServiceRequestProcessor
    {
        Task<ProcessingResponse> ProcessRequest(RequestData requestData);
    }
}
