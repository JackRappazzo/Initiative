using Initiative.Api.Core.Services.Bestiary;
using Initiative.Api.Extensions;
using Initiative.Api.Messages.Bestiary;
using Initiative.Persistence.Models.Bestiary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BestiaryController : ControllerBase
    {
        private readonly IBestiaryService _bestiaryService;

        public BestiaryController(IBestiaryService bestiaryService)
        {
            _bestiaryService = bestiaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableBestiaries(CancellationToken cancellationToken)
        {
            var bestiaries = await _bestiaryService.GetAvailableBestiaries(User.GetUserId()!, cancellationToken);

            return Ok(new GetAvailableBestiariesResponse
            {
                Bestiaries = bestiaries.Select(b => new GetAvailableBestiariesResponse.BestiaryItem
                {
                    Id = b.Id,
                    Name = b.Name,
                    Source = b.Source,
                    OwnerId = b.OwnerId
                })
            });
        }

        [HttpGet("creatures")]
        public async Task<IActionResult> SearchCreatures([FromQuery] SearchCreaturesRequest request, CancellationToken cancellationToken)
        {
            var query = new BestiarySearchQuery
            {
                BestiaryIds = request.BestiaryIds,
                NameSearch = request.Name,
                CreatureType = request.CreatureType,
                ChallengeRating = request.ChallengeRating,
                IsLegendary = request.IsLegendary,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending,
                PageSize = request.PageSize,
                Skip = request.Skip
            };

            var result = await _bestiaryService.SearchCreatures(query, cancellationToken);

            return Ok(new SearchCreaturesResponse
            {
                Creatures = result.Creatures.Select(c => new SearchCreaturesResponse.CreatureItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    BestiaryId = c.BestiaryId,
                    Source = c.Source,
                    CreatureType = c.CreatureType,
                    ChallengeRating = c.ChallengeRating,
                    IsLegendary = c.IsLegendary
                }),
                TotalCount = result.TotalCount
            });
        }

        [HttpGet("creatures/{creatureId}")]
        public async Task<IActionResult> GetCreatureById(string creatureId, CancellationToken cancellationToken)
        {
            var creature = await _bestiaryService.GetCreatureById(creatureId, cancellationToken);
            if (creature is null) return NotFound();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson };
            var rawJson = creature.RawData.ToJson(jsonWriterSettings);

            return Content(
                $$"""{ "id": "{{creature.Id}}", "name": {{System.Text.Json.JsonSerializer.Serialize(creature.Name)}}, "bestiaryId": "{{creature.BestiaryId}}", "source": {{System.Text.Json.JsonSerializer.Serialize(creature.Source)}}, "rawData": {{rawJson}} }""",
                "application/json"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateBestiary([FromBody] CreateBestiaryRequest request, CancellationToken cancellationToken)
        {
            var bestiary = await _bestiaryService.CreateCustomBestiary(User.GetUserId()!, request.Name, cancellationToken);
            return Ok(new { id = bestiary.Id, name = bestiary.Name });
        }

        [HttpPut("{bestiaryId}")]
        public async Task<IActionResult> RenameBestiary(string bestiaryId, [FromBody] RenameBestiaryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _bestiaryService.RenameBestiary(bestiaryId, User.GetUserId()!, request.Name, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{bestiaryId}")]
        public async Task<IActionResult> DeleteBestiary(string bestiaryId, CancellationToken cancellationToken)
        {
            try
            {
                await _bestiaryService.DeleteBestiary(bestiaryId, User.GetUserId()!, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost("{bestiaryId}/creatures")]
        public async Task<IActionResult> CreateCustomCreature(string bestiaryId, [FromBody] SaveCustomCreatureRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var creature = await _bestiaryService.CreateCustomCreature(bestiaryId, User.GetUserId()!, MapToCustomCreatureData(request), cancellationToken);
                return Ok(new { id = creature.Id, name = creature.Name });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPut("{bestiaryId}/creatures/{creatureId}")]
        public async Task<IActionResult> UpdateCustomCreature(string bestiaryId, string creatureId, [FromBody] SaveCustomCreatureRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _bestiaryService.UpdateCustomCreature(creatureId, bestiaryId, User.GetUserId()!, MapToCustomCreatureData(request), cancellationToken);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{bestiaryId}/creatures/{creatureId}")]
        public async Task<IActionResult> DeleteCustomCreature(string bestiaryId, string creatureId, CancellationToken cancellationToken)
        {
            await _bestiaryService.DeleteCustomCreature(creatureId, cancellationToken);
            return NoContent();
        }

        private static CustomCreatureData MapToCustomCreatureData(SaveCustomCreatureRequest request) => new()
        {
            Name = request.Name,
            Size = request.Size,
            CreatureType = request.CreatureType,
            Subtype = request.Subtype,
            Alignment = request.Alignment,
            ChallengeRating = request.ChallengeRating,
            IsLegendary = request.IsLegendary,
            ProficiencyBonus = request.ProficiencyBonus,
            HP = request.HP,
            HitDice = request.HitDice,
            AC = request.AC,
            AcNote = request.AcNote,
            AbilityScores = request.AbilityScores == null ? null : new()
            {
                Str = request.AbilityScores.Str,
                Dex = request.AbilityScores.Dex,
                Con = request.AbilityScores.Con,
                Int = request.AbilityScores.Int,
                Wis = request.AbilityScores.Wis,
                Cha = request.AbilityScores.Cha,
            },
            Speed = request.Speed == null ? null : new()
            {
                Walk = request.Speed.Walk,
                Fly = request.Speed.Fly,
                Swim = request.Speed.Swim,
                Burrow = request.Speed.Burrow,
                Climb = request.Speed.Climb,
                CanHover = request.Speed.CanHover,
            },
            SavingThrows = request.SavingThrows,
            Skills = request.Skills,
            DamageResistances = request.DamageResistances,
            DamageImmunities = request.DamageImmunities,
            DamageVulnerabilities = request.DamageVulnerabilities,
            ConditionImmunities = request.ConditionImmunities,
            Senses = request.Senses,
            Languages = request.Languages,
            Traits = request.Traits,
            Actions = request.Actions?.Select(e => new CustomCreatureEntry { Name = e.Name, Description = e.Description }).ToList(),
            BonusActions = request.BonusActions?.Select(e => new CustomCreatureEntry { Name = e.Name, Description = e.Description }).ToList(),
            Reactions = request.Reactions?.Select(e => new CustomCreatureEntry { Name = e.Name, Description = e.Description }).ToList(),
            LegendaryActions = request.LegendaryActions?.Select(e => new CustomCreatureEntry { Name = e.Name, Description = e.Description }).ToList(),
            LegendaryActionCount = request.LegendaryActionCount,
            Spellcasting = request.Spellcasting == null ? null : new()
            {
                Ability = request.Spellcasting.Ability,
                SpellSaveDc = request.Spellcasting.SpellSaveDc,
                SpellAttackBonus = request.Spellcasting.SpellAttackBonus,
                SlotSpells = request.Spellcasting.SlotSpells,
                DailySpells = request.Spellcasting.DailySpells,
                Description = request.Spellcasting.Description,
                FreeformText = request.Spellcasting.FreeformText,
            },
        };
    }
}
