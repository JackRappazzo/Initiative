using Initiative.Api.Core.Services.Condition;
using Initiative.Persistence.Models.Condition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConditionController : ControllerBase
    {
        private readonly IConditionService _conditionService;

        public ConditionController(IConditionService conditionService)
        {
            _conditionService = conditionService;
        }

        [HttpGet("conditions/resolve")]
        public async Task<IActionResult> ResolveCondition([FromQuery] string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("name is required.");

            var condition = await _conditionService.GetConditionByName(name, cancellationToken);
            if (condition is null) return NotFound();

            return Content(BuildConditionJson(condition), "application/json");
        }

        private static string BuildConditionJson(ConditionDocument condition)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson };
            var rawJson = condition.RawData.ToJson(jsonWriterSettings);

            return $$"""{"id":"{{condition.Id}}","name":{{System.Text.Json.JsonSerializer.Serialize(condition.Name)}},"source":{{System.Text.Json.JsonSerializer.Serialize(condition.Source)}},"type":{{System.Text.Json.JsonSerializer.Serialize(condition.Type)}},"rawData":{{rawJson}}}""";
        }
    }
}
