using System;
using ProjectV.Models.Users;

namespace ProjectV.Models.WebServices.Responses
{
    public sealed class ValidateRefreshTokenResponse : BaseResponse
    {
        public Guid UserId { get; set; }
        public UserId ConvertedUserId => Users.UserId.Wrap(UserId);


        public ValidateRefreshTokenResponse()
        {
        }
    }
}
