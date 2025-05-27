
using Initiative.Api.Core.Services.Encounters;
using Initiative.Api.Extensions;
using Initiative.Api.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncounterController : ControllerBase
    {
        protected readonly IEncounterService encounterService;

        public EncounterController(IEncounterService encounterService)
        {
            this.encounterService = encounterService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateEncounter(CreateEncounterRequest request, CancellationToken cancellationToken)
        {
            var newEncounterId = await encounterService.CreateEncounter(User.GetUserId(), request.EncounterName, cancellationToken);

            return Ok(new CreateEncounterResponse()
            {
                EncounterId = newEncounterId,
                EncounterName = request.EncounterName
            });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetEncounterList(CancellationToken cancellationToken)
        {
            var encounters = await encounterService.GetEncounterList(User.GetUserId(), cancellationToken);
            return Ok(encounters);
        }

    }
}
