using System;

namespace ProjectV.Models.WebServices.Responses
{
    public sealed class SignupResponse : BaseResponse
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = default!;


        public SignupResponse()
        {
        }
    }
}
