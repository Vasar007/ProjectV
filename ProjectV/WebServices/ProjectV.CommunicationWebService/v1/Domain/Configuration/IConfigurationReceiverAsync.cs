using System.Threading.Tasks;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Configuration
{
    public interface IConfigurationReceiverAsync
    {
        Task<StartJobDataResponce> ReceiveConfigForRequestAsync(StartJobParamsRequest jobParams);
    }
}
