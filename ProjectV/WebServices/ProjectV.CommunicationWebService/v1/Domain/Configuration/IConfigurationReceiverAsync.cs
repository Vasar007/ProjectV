using System.Threading.Tasks;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Configuration
{
    public interface IConfigurationReceiverAsync
    {
        Task<StartJobDataResponce> ReceiveConfigForRequestAsync(StartJobParamsRequest jobParams);
    }
}
