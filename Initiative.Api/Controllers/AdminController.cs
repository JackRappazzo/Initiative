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
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IUserRegistrationService registrationService;
        IUserLoginService loginService;
        IJwtService jwtService;

        public AdminController(IUserRegistrationService registrationService, IUserLoginService loginService, IJwtService jwtService)
        {
            this.registrationService = registrationService;
            this.loginService = loginService;
            this.jwtService = jwtService;
        }


        [Route("register")]
        [HttpPost]
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

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest data, CancellationToken cancellationToken)
        {
            (var isLoggedIn, var errorMessage, var token) = await loginService.LoginAndFetchToken(data.EmailAddress, data.Password, cancellationToken);

            if(isLoggedIn)
            {
                return Ok(new LoginResponse()
                {
                    Success = true,
                    Jwt = token
                });
            }
            else
            {
                return BadRequest(errorMessage);
            }
        }
    }
}
