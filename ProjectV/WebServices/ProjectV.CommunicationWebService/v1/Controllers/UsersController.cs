using System;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.CommonWebApi.Authorization.Tokens.Services;
using ProjectV.CommonWebApi.Authorization.Users.Services;
using ProjectV.CommonWebApi.Controllers;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public sealed class UsersController : ProjectVApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;


        public UsersController(
            IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService.ThrowIfNull(nameof(userService));
            _tokenService = tokenService.ThrowIfNull(nameof(tokenService));
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequest? loginRequest)
        {
            if (loginRequest is null ||
                string.IsNullOrEmpty(loginRequest.UserName) ||
                string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new TokenResponse
                {
                    Error = "Missing login details",
                    ErrorCode = "L01"
                });
            }

            var loginResponse = await _userService.LoginAsync(loginRequest);

            if (!loginResponse.Success)
            {
                return Unauthorized(new
                {
                    loginResponse.ErrorCode,
                    loginResponse.Error
                });
            }

            return Ok(loginResponse);
        }

        [HttpPost]
        [Route("refresh_token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest? refreshTokenRequest)
        {
            if (refreshTokenRequest is null ||
                string.IsNullOrEmpty(refreshTokenRequest.RefreshToken) ||
                refreshTokenRequest.UserId == Guid.Empty)
            {
                return BadRequest(new TokenResponse
                {
                    Error = "Missing refresh token details",
                    ErrorCode = "R01"
                });
            }

            var validateRefreshTokenResponse = await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);

            if (!validateRefreshTokenResponse.Success)
            {
                return UnprocessableEntity(validateRefreshTokenResponse);
            }

            var tokenResponse = await _tokenService.GenerateTokensAsync(validateRefreshTokenResponse.UserId);

            if (tokenResponse is null)
            {
                return UnprocessableEntity(validateRefreshTokenResponse);
            }

            return Ok(new
            {
                tokenResponse.AccessToken,
                tokenResponse.RefreshToken
            });
        }

        [HttpPost]
        [Route("signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup(SignupRequest signupRequest)
        {
            signupRequest.ThrowIfNull(nameof(signupRequest));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors.Select(c => c.ErrorMessage))
                    .ToReadOnlyList();

                if (errors.Any())
                {
                    return BadRequest(new TokenResponse
                    {
                        Error = $"{string.Join(",", errors)}",
                        ErrorCode = "S01"
                    });
                }
            }

            var signupResponse = await _userService.SignupAsync(signupRequest);

            if (!signupResponse.Success)
            {
                return UnprocessableEntity(signupResponse);
            }

            return Ok(signupResponse);
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var logout = await _userService.LogoutAsync(Uid);

            if (!logout.Success)
            {
                return UnprocessableEntity(logout);
            }

            return Ok();
        }
    }
}