using System;
using System.ComponentModel.DataAnnotations;
using ProjectV.Models.Authorization;

namespace ProjectV.Models.WebServices.Requests
{
    public sealed class SignupRequest
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        public DateTime Timestamp { get; set; }


        public SignupRequest()
        {
        }
    }
}
