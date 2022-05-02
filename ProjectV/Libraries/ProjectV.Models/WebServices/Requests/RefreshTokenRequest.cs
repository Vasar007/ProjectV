using System;

namespace ProjectV.Models.WebService.Requests
{
    // TODO: make this DTO immutable.
    public sealed class RefreshTokenRequest
    {
        public Guid UserId { get; set; }

        public string RefreshToken { get; set; } = default!;


        public RefreshTokenRequest()
        {
        }
    }
}
