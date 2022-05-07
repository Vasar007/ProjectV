using System;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Authorization.Tokens.Clients
{
    public interface ITokenClient : IDisposable
    {
        Task<Result<TokenResponse, ErrorResponse>> LoginAsync(LoginRequest login);
    }
}
