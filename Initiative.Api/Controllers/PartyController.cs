using Initiative.Api.Core.Services.Parties;
using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Api.Extensions;
using Initiative.Api.Messages.Parties;
using Initiative.Persistence.Models.Parties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartyController : ControllerBase
    {
        private readonly IPartyService partyService;
        private readonly ISidekickService sidekickService;

        public PartyController(IPartyService partyService, ISidekickService sidekickService)
        {
            this.partyService = partyService;
            this.sidekickService = sidekickService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateParty(CreatePartyRequest request, CancellationToken cancellationToken)
        {
            var members = request.Members.Select(m => MapMember(m));

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

            var response = parties.Select(ToFetchResponse);

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

            return Ok(ToFetchResponse(party));
        }

        [HttpPut("{partyId}/members"), Authorize]
        public async Task<IActionResult> UpdateMembers(string partyId, UpdatePartyMembersRequest request, CancellationToken cancellationToken)
        {
            var existing = await partyService.GetParty(partyId, User.GetUserId()!, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            var sidekicksByMemberId = existing.Members
                .Where(m => m.Sidekick != null)
                .ToDictionary(m => m.Id, m => m.Sidekick);

            var members = request.Members.Select(m =>
            {
                var member = MapMember(m);
                if (m.Id != null && sidekicksByMemberId.TryGetValue(m.Id, out var sidekick))
                {
                    member.Sidekick = sidekick;
                }
                return member;
            });

            var updated = await partyService.UpdatePartyMembers(partyId, User.GetUserId()!, members, cancellationToken);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{partyId}/sidekicks"), Authorize]
        public async Task<IActionResult> CreateSidekick(string partyId, SaveSidekickRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var member = await sidekickService.CreateSidekick(partyId, User.GetUserId()!, request.Name, MapSidekickData(request), cancellationToken);
                return Ok(new { memberId = member.Id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{partyId}/sidekicks/{memberId}"), Authorize]
        public async Task<IActionResult> UpdateSidekick(string partyId, string memberId, SaveSidekickRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await sidekickService.UpdateSidekick(partyId, User.GetUserId()!, memberId, request.Name, MapSidekickData(request), cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{partyId}/sidekicks/{memberId}"), Authorize]
        public async Task<IActionResult> DeleteSidekick(string partyId, string memberId, CancellationToken cancellationToken)
        {
            var removed = await sidekickService.RemoveSidekick(partyId, User.GetUserId()!, memberId, cancellationToken);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
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

        private static FetchPartyResponse ToFetchResponse(PartyDocument party) => new()
        {
            PartyId = party.Id,
            Name = party.Name,
            Members = party.Members.Select(m => new PartyMemberJsonModel
            {
                Id = m.Id,
                Name = m.Name,
                Level = m.Level,
                DndBeyondCharacterId = m.DndBeyondCharacterId,
                Sidekick = MapSidekick(m.Sidekick)
            })
        };

        private static PartyMember MapMember(PartyMemberJsonModel m) => new()
        {
            Id = string.IsNullOrWhiteSpace(m.Id) ? Guid.NewGuid().ToString() : m.Id,
            Name = m.Name,
            Level = m.Level,
            DndBeyondCharacterId = m.DndBeyondCharacterId
        };

        private static SidekickJsonModel? MapSidekick(Sidekick? sidekick)
        {
            if (sidekick == null)
            {
                return null;
            }

            return new SidekickJsonModel
            {
                BaseCreatureId = sidekick.BaseCreatureId,
                BaseCreatureName = sidekick.BaseCreatureName,
                Class = sidekick.Class.ToString(),
                Level = sidekick.Level,
                AbilityScores = sidekick.AbilityScores,
                MaxHitPoints = sidekick.MaxHitPoints,
                HitDiceCount = sidekick.HitDiceCount,
                HitDieSize = sidekick.HitDieSize,
                HpRolls = sidekick.HpRolls,
                Spellcasting = sidekick.Spellcasting == null ? null : new SidekickSpellcastingJsonModel
                {
                    Kind = sidekick.Spellcasting.Kind.ToString(),
                    Ability = sidekick.Spellcasting.Ability,
                    Cantrips = sidekick.Spellcasting.Cantrips,
                    Slots = sidekick.Spellcasting.Slots,
                    KnownSpells = sidekick.Spellcasting.KnownSpells
                },
                Choices = sidekick.Choices,
                RawData = BsonTypeMapper.MapToDotNetValue(sidekick.RawData)
            };
        }

        private static SidekickData MapSidekickData(SaveSidekickRequest request) => new()
        {
            BaseCreatureId = request.BaseCreatureId,
            BaseCreatureName = request.BaseCreatureName,
            Class = Enum.TryParse<SidekickClass>(request.Class, true, out var sidekickClass) ? sidekickClass : SidekickClass.Expert,
            Level = request.Level,
            AbilityScores = request.AbilityScores,
            HpRolls = request.HpRolls,
            SpellcasterKind = Enum.TryParse<SidekickSpellcasterKind>(request.SpellcasterKind ?? "", true, out var kind) ? kind : null,
            WarriorRole = Enum.TryParse<SidekickWarriorRole>(request.WarriorRole ?? "", true, out var role) ? role : null,
            Cantrips = request.Cantrips,
            KnownSpells = request.KnownSpells,
            Choices = request.Choices
        };
    }
}
