using System.Threading.Tasks;
using ProjectV.Models.Users;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommonWebApi.Authorization.Users.Services
{
    public interface IUserService
    {
        Task<SignupResponse> SignupAsync(SignupRequest signupRequest);
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<LogoutResponse> LogoutAsync(UserId userId);
    }
}
