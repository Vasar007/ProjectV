namespace ProjectV.Models.WebServices.Requests
{
    public sealed class LoginRequest
    {
        public string UserName { get; set; } = default!;

        public string Password { get; set; } = default!;


        public LoginRequest()
        {
        }
    }
}
