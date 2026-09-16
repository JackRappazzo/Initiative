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
                    TemporaryHP = character.TemporaryHP,

                    Strength = character.Strength,
                    Dexterity = character.Dexterity,
                    Constitution = character.Constitution,
                    Intelligence = character.Intelligence,
                    Wisdom = character.Wisdom,
                    Charisma = character.Charisma,

                    ArmorClass = character.ArmorClass,
                    ProficiencyBonus = character.ProficiencyBonus,
                    Race = character.Race,
                    Speed = character.Speed,

                    SpellSlots = character.SpellSlots
                        .Select(s => new GetDndBeyondCharacterResponse.SpellSlot
                        {
                            Level = s.Level,
                            Used = s.Used,
                            Available = s.Available
                        })
                        .ToList(),
                    PactSlots = character.PactSlots
                        .Select(s => new GetDndBeyondCharacterResponse.SpellSlot
                        {
                            Level = s.Level,
                            Used = s.Used,
                            Available = s.Available
                        })
                        .ToList(),
                    PreparedSpells = character.PreparedSpells
                        .Select(p => new GetDndBeyondCharacterResponse.PreparedSpell
                        {
                            Name = p.Name,
                            Level = p.Level,
                            IsCantrip = p.IsCantrip
                        })
                        .ToList(),
                    FocusPoints = character.FocusPoints is null
                        ? null
                        : new GetDndBeyondCharacterResponse.Resource
                        {
                            Current = character.FocusPoints.Current,
                            Max = character.FocusPoints.Max
                        },
                    Attacks = character.Attacks
                        .Select(a => new GetDndBeyondCharacterResponse.Attack
                        {
                            Name = a.Name,
                            ToHitBonus = a.ToHitBonus,
                            DamageDice = a.DamageDice,
                            DamageBonus = a.DamageBonus,
                            DamageType = a.DamageType,
                            Range = a.Range,
                            LongRange = a.LongRange,
                            IsRanged = a.IsRanged
                        })
                        .ToList()
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
