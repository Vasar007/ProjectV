using ProjectV.Models.Authorization.Tokens;

namespace ProjectV.Models.WebServices.Responses
{
    public sealed class TokenResponse : BaseResponse
    {
        public AccessTokenData AccessToken { get; set; } = default!;
        public RefreshTokenData RefreshToken { get; set; } = default!;


        public TokenResponse()
        {
        }
    }
}
