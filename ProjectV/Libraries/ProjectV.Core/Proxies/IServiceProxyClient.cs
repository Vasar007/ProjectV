using System;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Proxies
{
    public interface IServiceProxyClient : IDisposable
    {
        Task<Result<ProcessingResponse, ErrorResponse>> SendRequest(StartJobParamsRequest jobParams);
    }
}
