using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Initiative.Api.Messages;
using Initiative.Api.Core.Identity;
using System.Threading.Tasks;
using Initiative.Api.Core.Services.Authentication;
using Initiative.Api.Core.Services.Users;

using Microsoft.AspNetCore.Authorization;
using Initiative.Api.Core.Services.Authentication;

namespace Initiative.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IUserService registrationService;
        IUserLoginService loginService;
        IJwtRefreshService jwtRefreshService;

        public AdminController(IUserService registrationService, IUserLoginService loginService, IJwtRefreshService jwtRefreshService)
        {
            this.registrationService = registrationService;
            this.loginService = loginService;
            this.jwtRefreshService = jwtRefreshService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest data, CancellationToken cancellationToken)
        {
            (bool success, string message) = await registrationService.RegisterUser(data.DisplayName, data.EmailAddress, data.Password, cancellationToken);
            
            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest data, CancellationToken cancellationToken)
        {
            LoginResult result = await loginService.LoginAndFetchTokens(data.EmailAddress, data.Password, cancellationToken);

            if(result.Success)
            {
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions()
                {
                    Expires = DateTime.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });

                return Ok(new LoginResponse()
                {
                    Success = true,
                    Jwt = result.Jwt,
                });
               
            }
            else
            {
                string errorMessage;
                switch(result.ErrorType)
                {
                    case LoginErrorType.EmailDoesNotExist:
                        errorMessage = "The given email does not exist";
                        break;
                    case LoginErrorType.PasswordMismatch:
                        errorMessage = "The password does not match";
                        break;
                    case LoginErrorType.Unknown:
                    default:
                        errorMessage = "An unknown error has occurred";
                        break;
                }

                return BadRequest(errorMessage);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"] as string;

            (var success, var token) = await jwtRefreshService.RefreshJwt(refreshToken, cancellationToken);

            if(success)
            {
                return Ok(new RefreshJwtResponse()
                {
                    AccessToken = token
                });
            }
            else
            {
                return BadRequest("Access denied. Log in again");
            }
        }

        [Authorize]
        [HttpGet("isLoggedIn")]
        public async Task<IActionResult> IsLoggedIn(CancellationToken cancellationToken)
        {
            return Ok("Authorized!");
        }
    }
}
