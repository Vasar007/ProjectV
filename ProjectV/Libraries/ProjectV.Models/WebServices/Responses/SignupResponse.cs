namespace ProjectV.Models.WebServices.Responses
{
    public sealed class SignupResponse : BaseResponse
    {
        public string UserName { get; set; } = default!;


        public SignupResponse()
        {
        }
    }
}
