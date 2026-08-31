using System.Net;
using Initiative.Api.Core.Services.DndBeyond;
using Initiative.Api.Messages.DndBeyond;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DndBeyondController : ControllerBase
    {
        private const string TokenHeader = "X-DndBeyond-Token";
        private readonly IDndBeyondProxyService dndBeyondProxyService;

        public DndBeyondController(IDndBeyondProxyService dndBeyondProxyService)
        {
            this.dndBeyondProxyService = dndBeyondProxyService;
        }

        [HttpGet("characters")]
        public async Task<IActionResult> GetCharacters(CancellationToken cancellationToken)
        {
            var token = GetToken();
            if (token is null)
            {
                return BadRequest("D&D Beyond session token is required.");
            }

            try
            {
                var characters = await dndBeyondProxyService.GetCharacters(token, cancellationToken);

                return Ok(new GetDndBeyondCharactersResponse
                {
                    Characters = characters.Select(c => new GetDndBeyondCharactersResponse.DndBeyondCharacterItem
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                });
            }
            catch (DndBeyondRequestException ex)
            {
                return MapError(ex);
            }
        }

        [HttpGet("characters/{characterId}")]
        public async Task<IActionResult> GetCharacter(string characterId, CancellationToken cancellationToken)
        {
            var token = GetToken();
            if (token is null)
            {
                return BadRequest("D&D Beyond session token is required.");
            }

            try
            {
                var character = await dndBeyondProxyService.GetCharacter(token, characterId, cancellationToken);
                if (character is null)
                {
                    return NotFound();
                }

                return Ok(new GetDndBeyondCharacterResponse
                {
                    Id = character.Id,
                    Name = character.Name,
                    Level = character.Level,
                    ClassName = character.ClassName,
                    MaxHP = character.MaxHP,
                    CurrentHP = character.CurrentHP,
                    TemporaryHP = character.TemporaryHP
                });
            }
            catch (DndBeyondRequestException ex)
            {
                return MapError(ex);
            }
        }

        private string? GetToken()
        {
            if (Request.Headers.TryGetValue(TokenHeader, out var values))
            {
                var token = values.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    return token;
                }
            }

            return null;
        }

        private IActionResult MapError(DndBeyondRequestException exception)
        {
            if (exception.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return Unauthorized(exception.Message);
            }

            if (exception.RetryAfter.HasValue)
            {
                var retryAfterSeconds = (int)Math.Ceiling(exception.RetryAfter.Value.TotalSeconds);
                Response.Headers["Retry-After"] = retryAfterSeconds.ToString();

                return StatusCode((int)exception.StatusCode, new
                {
                    message = exception.Message,
                    retryAfterSeconds
                });
            }

            return StatusCode((int)exception.StatusCode, exception.Message);
        }
    }
}
