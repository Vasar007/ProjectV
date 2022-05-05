using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.CommonWebApi.Authorization.Tokens.Services;
using ProjectV.CommonWebApi.Authorization.Users.Services;
using ProjectV.CommonWebApi.Controllers;
using ProjectV.CommonWebApi.Extensions;
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            (bool isValid, string? error) = ModelState.ValidateModel();
            if (!isValid)
            {
                if (string.IsNullOrWhiteSpace(error))
                    error = "Missing login details";

                return BadRequest(new TokenResponse
                {
                    ErrorMessage = error,
                    ErrorCode = "L01"
                });
            }

            var loginResponse = await _userService.LoginAsync(loginRequest);
            if (!loginResponse.Success)
            {
                return Unauthorized(loginResponse);
            }

            return Ok(loginResponse);
        }

        [HttpPost]
        [Route("refresh_token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            (bool isValid, string? error) = ModelState.ValidateModel();
            if (!isValid || !refreshTokenRequest.HasAnyUserInfo())
            {
                if (string.IsNullOrWhiteSpace(error))
                    error = "Missing refresh token details";

                return BadRequest(new SignupResponse
                {
                    ErrorMessage = error,
                    ErrorCode = "R01"
                });
            }

            var validateRefreshTokenResponse = await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);
            if (!validateRefreshTokenResponse.Success)
            {
                return UnprocessableEntity(validateRefreshTokenResponse);
            }

            var tokenResponse = await _tokenService.GenerateTokensAsync(validateRefreshTokenResponse.ConvertedUserId);
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
            (bool isValid, string? error) = ModelState.ValidateModel();
            if (!isValid)
            {
                if (string.IsNullOrWhiteSpace(error))
                    error = "Missing sign up details";

                return BadRequest(new SignupResponse
                {
                    ErrorMessage = error,
                    ErrorCode = "S01"
                });
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