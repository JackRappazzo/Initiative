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

            var creatures = await _bestiaryService.SearchCreatures(query, cancellationToken);

            return Ok(new SearchCreaturesResponse
            {
                Creatures = creatures.Select(c => new SearchCreaturesResponse.CreatureItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    BestiaryId = c.BestiaryId,
                    Source = c.Source,
                    CreatureType = c.CreatureType,
                    ChallengeRating = c.ChallengeRating,
                    IsLegendary = c.IsLegendary
                })
            });
        }

        [HttpGet("creatures/count")]
        public async Task<IActionResult> CountCreatures([FromQuery] SearchCreaturesRequest request, CancellationToken cancellationToken)
        {
            var query = new BestiarySearchQuery
            {
                BestiaryIds = request.BestiaryIds,
                NameSearch = request.Name,
                CreatureType = request.CreatureType,
                ChallengeRating = request.ChallengeRating,
                IsLegendary = request.IsLegendary
            };

            var totalCount = await _bestiaryService.CountCreatures(query, cancellationToken);

            return Ok(new CountCreaturesResponse { TotalCount = totalCount });
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
    }
}
