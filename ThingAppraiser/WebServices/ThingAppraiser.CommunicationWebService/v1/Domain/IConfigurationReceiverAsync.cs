using System.Threading.Tasks;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public interface IConfigurationReceiverAsync
    {
        Task<RequestData> ReceiveConfigForRequestAsync(RequestParams requestParams);
    }
}
