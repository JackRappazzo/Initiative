
using Initiative.Api.Core.Services.Encounters;
using Initiative.Api.Extensions;
using Initiative.Api.Messages;
using Initiative.Persistence.Models.Encounters;
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
                DisplayName = request.EncounterName
            });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetEncounterList(CancellationToken cancellationToken)
        {
            var encounters = await encounterService.GetEncounterList(User.GetUserId(), cancellationToken);
            return Ok(encounters);
        }

        [HttpGet("{encounterId}"), Authorize]
        public async Task<IActionResult> GetEncounter(string encounterId, CancellationToken cancellationToken)
        {
            var encounter = await encounterService.GetEncounter(encounterId, User.GetUserId(), cancellationToken);
            if (encounter == null)
            {
                return NotFound();
            }

            var response = new FetchEncounterResponse()
            {
                DisplayName = encounter.DisplayName,
                EncounterId = encounter.Id,
                CreatedAt = encounter.CreatedAt,
                Creatures = encounter.Creatures.Select(c => new CreatureJsonModel()
                {
                    Name = c.Name,
                    HitPoints = c.HitPoints,
                    MaximumHitPoints = c.MaximumHitPoints,
                    ArmorClass = c.ArmorClass,
                    Initiative = c.Initiative,
                    InitiativeModifier = c.InitiativeModifier
                }).ToList()
            };
            return Ok(response);
        }

        [HttpPost("{encounterId}/creatures"), Authorize]
        public async Task<IActionResult> SetCreatures(string encounterId, [FromBody] IEnumerable<CreatureJsonModel> creatures, CancellationToken cancellationToken)
        {
            if (creatures == null)
            {
                return BadRequest("Creatures must not be null. Send empty array to set to no creatures");
            }
            var creaturesToSet = creatures.Select(c =>
                new Creature()
                {
                    Name = c.Name,
                    HitPoints = c.HitPoints,
                    MaximumHitPoints = c.MaximumHitPoints,
                    ArmorClass = c.ArmorClass,
                    Initiative = c.Initiative,
                    InitiativeModifier = c.InitiativeModifier,
                }).ToList();

            await encounterService.SetEncounterCreatures(encounterId, User.GetUserId(), creaturesToSet, cancellationToken);
            return NoContent();
        }

        [HttpPut("{encounterId}/setName"), Authorize]
        public async Task<IActionResult> RenameEncounter(string encounterId, [FromBody] SetEncounterNameRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewName))
            {
                return BadRequest("Encounter name must not be empty.");
            }

            await encounterService.RenameEncounter(encounterId, User.GetUserId(), request.NewName, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{encounterId}"), Authorize]
        public async Task<IActionResult> DeleteEncounter(string encounterId, CancellationToken cancellationToken)
        {
            var result = await encounterService.DeleteEncounter(encounterId, cancellationToken);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
