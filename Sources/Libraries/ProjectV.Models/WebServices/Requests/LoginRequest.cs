using System.ComponentModel.DataAnnotations;

namespace ProjectV.Models.WebServices.Requests
{
    public sealed class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;


        public LoginRequest()
        {
        }
    }
}
