using System;
using System.Threading.Tasks;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.Core.Proxies
{
    public interface IServiceProxy : IDisposable
    {
        Task<ProcessingResponse?> SendRequest(StartJobParamsRequest jobParams);
    }
}
