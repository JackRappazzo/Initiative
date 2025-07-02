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
using Initiative.Api.Extensions;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class UserController : ControllerBase
    {
        private readonly IInitiativeUserRepository userRepository;

        public UserController(IInitiativeUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet, Route("information")]
        public async Task<IActionResult> GetUserInformation(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            var user = await userRepository.FetchUserByIdentityId(userId, cancellationToken);

            if(user == null)
            {
                return NotFound("User not found.");
            }

            var response = new GetUserResponse()
            {
                DisplayName = user.EmailAddress,
                RoomCode = user.RoomCode,
            };

            return Ok(response);
        }
    }
}
