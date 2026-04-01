using Initiative.Api.Core.Services.Parties;
using Initiative.Api.Extensions;
using Initiative.Api.Messages.Parties;
using Initiative.Persistence.Models.Parties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartyController : ControllerBase
    {
        private readonly IPartyService partyService;

        public PartyController(IPartyService partyService)
        {
            this.partyService = partyService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateParty(CreatePartyRequest request, CancellationToken cancellationToken)
        {
            var members = request.Members.Select(m => new PartyMember
            {
                Name = m.Name,
                Level = m.Level
            });

            var newPartyId = await partyService.CreateParty(User.GetUserId()!, request.Name, members, cancellationToken);

            return Ok(new CreatePartyResponse
            {
                PartyId = newPartyId,
                Name = request.Name
            });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetParties(CancellationToken cancellationToken)
        {
            var parties = await partyService.GetParties(User.GetUserId()!, cancellationToken);

            var response = parties.Select(p => new FetchPartyResponse
            {
                PartyId = p.Id,
                Name = p.Name,
                Members = p.Members.Select(m => new PartyMemberJsonModel
                {
                    Name = m.Name,
                    Level = m.Level
                })
            });

            return Ok(response);
        }

        [HttpGet("{partyId}"), Authorize]
        public async Task<IActionResult> GetParty(string partyId, CancellationToken cancellationToken)
        {
            var party = await partyService.GetParty(partyId, User.GetUserId()!, cancellationToken);

            if (party == null)
            {
                return NotFound();
            }

            return Ok(new FetchPartyResponse
            {
                PartyId = party.Id,
                Name = party.Name,
                Members = party.Members.Select(m => new PartyMemberJsonModel
                {
                    Name = m.Name,
                    Level = m.Level
                })
            });
        }

        [HttpDelete("{partyId}"), Authorize]
        public async Task<IActionResult> DeleteParty(string partyId, CancellationToken cancellationToken)
        {
            var deleted = await partyService.DeleteParty(partyId, User.GetUserId()!, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
