using Initiative.Api.Core.Services.Bestiaries;
using Initiative.Api.Extensions;
using Initiative.Api.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestiaryController : ControllerBase
    {
        IBestiaryService bestiaryService;

        public BestiaryController(IBestiaryService bestiaryService)
        {
            this.bestiaryService = bestiaryService;
        }


        [HttpGet("/bestiaries/available"), Authorize]
        public async Task<IActionResult> GetAvailableBestiaries(CancellationToken cancellationToken)
        {
            var userId = Request.HttpContext.User.GetUserId();

            var bestiaries = await bestiaryService.GetAvailableBestiaries(userId, cancellationToken);

            var response = bestiaries.Select(b => new GetBestiaryStubResponse()
            { 
                Id = b.Id, 
                Name = b.Name, 
                IsPublic = b.OwnerId == null 
            });

            return Ok(response);
        }
    }
}
