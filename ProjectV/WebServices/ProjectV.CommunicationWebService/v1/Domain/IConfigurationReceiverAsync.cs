using System.Threading.Tasks;
using ProjectV.Models.WebService;

namespace ProjectV.CommunicationWebService.v1.Domain
{
    public interface IConfigurationReceiverAsync
    {
        Task<RequestData> ReceiveConfigForRequestAsync(RequestParams requestParams);
    }
}
