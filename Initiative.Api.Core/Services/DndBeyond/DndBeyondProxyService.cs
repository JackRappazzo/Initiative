using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondProxyService : IDndBeyondProxyService
    {
        private readonly HttpClient httpClient;

        public DndBeyondProxyService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<DndBeyondCharacterListItem>> GetCharacters(string token, CancellationToken cancellationToken)
        {
            var userId = ExtractUserId(token);
            var relativeUrl = userId.HasValue ? $"characters?userId={userId.Value}" : "characters";

            using var document = await GetJsonDocument(relativeUrl, token, cancellationToken);

            if (!document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var result = new List<DndBeyondCharacterListItem>();
            foreach (var item in data.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                result.Add(new DndBeyondCharacterListItem
                {
                    Id = GetLong(item, "id"),
                    Name = GetString(item, "characterName") ?? GetString(item, "name")
                });
            }

            return result;
        }

        public async Task<DndBeyondCharacterDetail?> GetCharacter(string token, string characterId, CancellationToken cancellationToken)
        {
            using var document = await GetJsonDocument($"character/{Uri.EscapeDataString(characterId)}", token, cancellationToken);

            if (!document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var overrideHp = GetNullableInt(data, "overrideHitPoints");
            var baseHp = GetInt(data, "baseHitPoints");
            var bonusHp = GetInt(data, "bonusHitPoints");
            var removedHp = GetInt(data, "removedHitPoints");

            var maxHp = overrideHp ?? baseHp + bonusHp;
            var currentHp = Math.Max(maxHp - removedHp, 0);

            return new DndBeyondCharacterDetail
            {
                Id = GetLong(data, "id"),
                Name = GetString(data, "characterName") ?? GetString(data, "name"),
                Level = GetLevel(data),
                ClassName = GetClassName(data),
                MaxHP = maxHp,
                CurrentHP = currentHp,
                TemporaryHP = GetInt(data, "temporaryHitPoints")
            };
        }

        private async Task<JsonDocument> GetJsonDocument(string relativeUrl, string token, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new DndBeyondRequestException(response.StatusCode, "D&D Beyond session token is invalid or has expired.");
            }

            if (!response.IsSuccessStatusCode)
            {
                TimeSpan? retryAfter = null;
                if (response.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable)
                {
                    retryAfter = ParseRetryAfter(response);
                }

                throw new DndBeyondRequestException(
                    response.StatusCode,
                    $"D&D Beyond request failed with status {(int)response.StatusCode}.",
                    retryAfter);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        }

        private static long? ExtractUserId(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var userId = jwt.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                             ?? jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (long.TryParse(userId, out var parsed))
                {
                    return parsed;
                }
            }
            catch
            {
                // Not a readable JWT; caller will fall back to a userId-less request.
            }

            return null;
        }

        private static TimeSpan? ParseRetryAfter(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                var value = values.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (int.TryParse(value, out var seconds))
                    {
                        return TimeSpan.FromSeconds(Math.Max(0, seconds));
                    }

                    if (DateTimeOffset.TryParse(value, out var date))
                    {
                        var delay = date - DateTimeOffset.UtcNow;
                        if (delay > TimeSpan.Zero)
                        {
                            return delay;
                        }
                    }
                }
            }

            return null;
        }

        private static int GetLevel(JsonElement data)
        {
            var level = 0;

            if (data.TryGetProperty("classes", out var classes) && classes.ValueKind == JsonValueKind.Array)
            {
                foreach (var classEntry in classes.EnumerateArray())
                {
                    level += GetInt(classEntry, "level");
                }
            }

            if (level == 0)
            {
                level = GetInt(data, "level");
            }

            return level;
        }

        private static string? GetClassName(JsonElement data)
        {
            var names = new List<string>();

            if (data.TryGetProperty("classes", out var classes) && classes.ValueKind == JsonValueKind.Array)
            {
                foreach (var classEntry in classes.EnumerateArray())
                {
                    var name = GetString(GetObject(classEntry, "definition"), "name");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names.Add(name!);
                    }
                }
            }

            return names.Count > 0 ? string.Join(", ", names) : null;
        }

        private static JsonElement GetObject(JsonElement element, string property)
        {
            return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Object
                ? value
                : default;
        }

        private static string? GetString(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }

            return null;
        }

        private static long GetLong(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var value))
            {
                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var result))
                {
                    return result;
                }

                if (value.ValueKind == JsonValueKind.String && long.TryParse(value.GetString(), out var parsed))
                {
                    return parsed;
                }
            }

            return 0;
        }

        private static int GetInt(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return 0;
            }

            return GetNullableInt(element, property) ?? 0;
        }

        private static int? GetNullableInt(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (!element.TryGetProperty(property, out var value) || value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var result))
            {
                return result;
            }

            return null;
        }
    }
}
