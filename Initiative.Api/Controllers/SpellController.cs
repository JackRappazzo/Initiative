using Initiative.Api.Core.Services.Spell;
using Initiative.Api.Messages.Spell;
using Initiative.Persistence.Models.Spell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SpellController : ControllerBase
    {
        private readonly ISpellService _spellService;

        public SpellController(ISpellService spellService)
        {
            _spellService = spellService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSpellSources(CancellationToken cancellationToken)
        {
            var sources = await _spellService.GetAvailableSpellSources(cancellationToken);

            return Ok(new GetAvailableSpellSourcesResponse
            {
                SpellSources = sources.Select(s => new GetAvailableSpellSourcesResponse.SpellSourceItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    Source = s.Source
                })
            });
        }

        [HttpGet("spells")]
        public async Task<IActionResult> SearchSpells([FromQuery] SearchSpellsRequest request, CancellationToken cancellationToken)
        {
            var query = new SpellSearchQuery
            {
                SpellSourceIds = request.SpellSourceIds,
                NameSearch = request.Name,
                School = request.School,
                PageSize = request.PageSize,
                Skip = request.Skip
            };

            var result = await _spellService.SearchSpells(query, cancellationToken);

            return Ok(new SearchSpellsResponse
            {
                Spells = result.Spells.Select(s => new SearchSpellsResponse.SpellItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    SpellSourceId = s.SpellSourceId,
                    Source = s.Source,
                    School = s.School
                }),
                TotalCount = result.TotalCount
            });
        }

        [HttpGet("spells/count")]
        public async Task<IActionResult> CountSpells([FromQuery] SearchSpellsRequest request, CancellationToken cancellationToken)
        {
            var query = new SpellSearchQuery
            {
                SpellSourceIds = request.SpellSourceIds,
                NameSearch = request.Name,
                School = request.School,
            };

            var count = await _spellService.SearchSpells(query, cancellationToken);
            return Ok(new { TotalCount = count.TotalCount });
        }

        [HttpGet("spells/{spellId}")]
        public async Task<IActionResult> GetSpellById(string spellId, CancellationToken cancellationToken)
        {
            var spell = await _spellService.GetSpellById(spellId, cancellationToken);
            if (spell is null) return NotFound();

            return Content(BuildSpellJson(spell), "application/json");
        }

        [HttpGet("spells/resolve")]
        public async Task<IActionResult> ResolveSpell([FromQuery] string name, [FromQuery] string? source, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("name is required.");

            var spell = await _spellService.GetSpellByNameAndSource(name, source, cancellationToken);
            if (spell is null) return NotFound();

            return Content(BuildSpellJson(spell), "application/json");
        }

        private static string BuildSpellJson(SpellDocument spell)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson };
            var rawJson = spell.RawData.ToJson(jsonWriterSettings);

            return $$"""{ "id": "{{spell.Id}}", "name": {{System.Text.Json.JsonSerializer.Serialize(spell.Name)}}, "spellSourceId": "{{spell.SpellSourceId}}", "source": {{System.Text.Json.JsonSerializer.Serialize(spell.Source)}}, "school": {{System.Text.Json.JsonSerializer.Serialize(spell.School)}}, "rawData": {{rawJson}} }""";
        }
    }
}
