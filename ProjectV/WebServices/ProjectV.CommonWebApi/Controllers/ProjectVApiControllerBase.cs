using Microsoft.AspNetCore.Mvc;
using ProjectV.Models.Users;
using System.Security.Claims;

namespace ProjectV.CommonWebApi.Controllers
{
    public abstract class ProjectVApiControllerBase : ControllerBase
    {
        protected UserId Uid => TryParseUserId();


        protected ProjectVApiControllerBase()
        {
        }

        private Claim? FindClaim(string claimName)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            Claim? claim = claimsIdentity?.FindFirst(claimName);
            return claim;
        }

        private UserId TryParseUserId()
        {
            var claim = FindClaim(ClaimTypes.NameIdentifier);
            if (UserId.TryParse(claim?.Value, out UserId userId))
            {
                return userId;
            }

            return UserId.None;
        }
    }
}
