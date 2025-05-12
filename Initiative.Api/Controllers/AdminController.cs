using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Initiative.Api.Messages;
using Initiative.Api.Core.Identity;
using System.Threading.Tasks;
using Initiative.Api.Services;
using Initiative.Api.Core.Authentication;

namespace Initiative.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IUserRegistrationService registrationService;
        IUserLoginService loginService;
        IJwtRefreshService jwtRefreshService;

        public AdminController(IUserRegistrationService registrationService, IUserLoginService loginService, IJwtRefreshService jwtRefreshService)
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
                return Ok(new LoginResponse()
                {
                    Success = true,
                    Jwt = result.Jwt,
                    RefreshToken = result.RefreshToken
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
        public async Task<IActionResult> Refresh([FromBody] RefreshJwtRequest request, CancellationToken cancellationToken)
        {
            (var success, var token) = await jwtRefreshService.RefreshJwt(request.RefreshToken, cancellationToken);

            if(success)
            {
                return Ok(new RefreshJwtResponse()
                {
                    RefreshToken = token
                });
            }
            else
            {
                return BadRequest("Access denied. Log in again");
            }
        }
    }
}
