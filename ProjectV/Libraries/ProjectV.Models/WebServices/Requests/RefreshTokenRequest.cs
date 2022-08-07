using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectV.Models.WebServices.Requests
{
    // TODO: make this DTO immutable.
    public sealed class RefreshTokenRequest
    {
        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        [Required]
        public string RefreshToken { get; set; } = default!;


        public RefreshTokenRequest()
        {
        }

        public bool HasAnyUserInfo()
        {
            return UserId != Guid.Empty ||
                   !string.IsNullOrWhiteSpace(UserName);
        }
    }
}
