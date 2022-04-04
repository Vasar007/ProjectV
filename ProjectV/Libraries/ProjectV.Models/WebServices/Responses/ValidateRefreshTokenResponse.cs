using ProjectV.Models.Users;

namespace ProjectV.Models.WebServices.Responses
{
    public sealed class ValidateRefreshTokenResponse : BaseResponse
    {
        public UserId UserId { get; set; }


        public ValidateRefreshTokenResponse()
        {
        }
    }
}
