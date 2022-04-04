namespace ProjectV.Models.WebServices.Responses
{
    public sealed class TokenResponse : BaseResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;


        public TokenResponse()
        {
        }
    }
}
