using Finerd.Api.Hubs;
using Finerd.Api.Model;
using Finerd.Api.Model.Responses;
using Finerd.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly ILogger<UsersController> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHubContext;

        public UsersController(IUserService userService, ITokenService tokenService, ILogger<UsersController> logger, IHubContext<NotificationHub, INotificationClient> notificationHubContext)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            _logger = logger;
            _notificationHubContext = notificationHubContext;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new TokenResponse
                {
                    Error = "Missing login details",
                    ErrorCode = "L01"
                });
            }

            var loginResponse = await userService.LoginAsync(loginRequest);

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
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken) || refreshTokenRequest.UserId == 0)
            {
                return BadRequest(new TokenResponse
                {
                    Error = "Missing refresh token details",
                    ErrorCode = "R01"
                });
            }
            var validateRefreshTokenResponse = await tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);
            if (!validateRefreshTokenResponse.Success)
            {
                return UnprocessableEntity(validateRefreshTokenResponse);
            }

            var tokenResponse = await tokenService.GenerateTokensAsync(validateRefreshTokenResponse.UserId);
            return Ok(new { AccessToken = tokenResponse.Item1, Refreshtoken = tokenResponse.Item2 });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signup")]
        public async Task<IActionResult> Signup(SignupRequest signupRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                if (errors.Any())
                {
                    return BadRequest(new TokenResponse
                    {
                        Error = $"{string.Join(",", errors)}",
                        ErrorCode = "S01"
                    });
                }
            }
            var signupResponse = await userService.SignupAsync(signupRequest);
            if (!signupResponse.Success)
            {
                return UnprocessableEntity(signupResponse);
            }
            await _notificationHubContext.Clients.Clients($"{signupResponse.Id}").ReceiveMessage("privateMessage", "You account was created. Please confirm your email");

            return Ok(signupResponse.Email);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserDto profile)
        {
            if (id != profile.Id)
                return BadRequest($"{nameof(id)}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                if (errors.Any())
                {
                    return BadRequest(new TokenResponse
                    {
                        Error = $"{string.Join(",", errors)}",
                        ErrorCode = "S01"
                    });
                }
            }
            var signupResponse = await userService.ProfileAsync(profile);
            if (!signupResponse.Success)
            {
                return UnprocessableEntity(signupResponse);
            }
            return Ok(signupResponse.Email);
        }


        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var logout = await userService.LogoutAsync(UserID);
            if (!logout.Success)
            {
                return UnprocessableEntity(logout);
            }
            return Ok();
        }
    }
}
