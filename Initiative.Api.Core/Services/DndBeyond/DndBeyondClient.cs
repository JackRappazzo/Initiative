using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using RestSharp;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondClient
    {
        private const string AuthServiceUrl = "https://auth-service.dndbeyond.com/v1/cobalt-token";
        private static readonly TimeSpan BearerTokenTtl = TimeSpan.FromMinutes(5);

        private readonly RestClient restClient;
        private readonly ConcurrentDictionary<string, CacheEntry> bearerCache = new();

        public DndBeyondClient(RestClient restClient)
        {
            this.restClient = restClient;
        }

        public Task<JsonDocument> GetCharactersAsync(string cobaltToken, CancellationToken cancellationToken)
            => ExecuteWithAuthRetryAsync(cobaltToken, cancellationToken, async (bearerToken) =>
                await ExecuteGetAsync("characters", bearerToken, cancellationToken) ?? JsonDocument.Parse("{}"));

        public Task<JsonDocument?> GetCharacterAsync(string cobaltToken, string characterId, CancellationToken cancellationToken)
            => ExecuteWithAuthRetryAsync(cobaltToken, cancellationToken, (bearerToken) =>
                ExecuteGetAsync($"character/{Uri.EscapeDataString(characterId)}", bearerToken, cancellationToken));

        private async Task<T> ExecuteWithAuthRetryAsync<T>(string cobaltToken, CancellationToken cancellationToken, Func<string, Task<T>> action)
        {
            var bearerToken = await GetBearerTokenAsync(cobaltToken, false, cancellationToken);

            try
            {
                return await action(bearerToken);
            }
            catch (DndBeyondRequestException ex) when (IsAuthFailure(ex))
            {
                bearerToken = await GetBearerTokenAsync(cobaltToken, true, cancellationToken);
                return await action(bearerToken);
            }
        }

        private async Task<string> GetBearerTokenAsync(string cobaltToken, bool forceRefresh, CancellationToken cancellationToken)
        {
            if (!forceRefresh && bearerCache.TryGetValue(cobaltToken, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
            {
                return cached.Token;
            }

            var request = new RestRequest(AuthServiceUrl, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", $"CobaltSession={cobaltToken}");

            var response = await restClient.ExecuteAsync(request, cancellationToken);

            if (response.ErrorException != null)
            {
                throw new DndBeyondRequestException(HttpStatusCode.BadGateway, "Failed to reach D&D Beyond.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new DndBeyondRequestException(response.StatusCode, "Failed to exchange D&D Beyond cobalt token for a bearer token.");
            }

            var bearerToken = ParseBearerToken(response.Content);
            if (bearerToken is null)
            {
                throw new DndBeyondRequestException(HttpStatusCode.BadGateway, "D&D Beyond bearer token response was malformed.");
            }

            bearerCache[cobaltToken] = new CacheEntry(bearerToken, DateTimeOffset.UtcNow + BearerTokenTtl);
            return bearerToken;
        }

        private async Task<JsonDocument?> ExecuteGetAsync(string resource, string bearerToken, CancellationToken cancellationToken)
        {
            var request = new RestRequest(resource, Method.Get);
            request.AddHeader("Authorization", $"Bearer {bearerToken}");

            var response = await restClient.ExecuteAsync(request, cancellationToken);

            if (response.ErrorException != null)
            {
                throw new DndBeyondRequestException(HttpStatusCode.BadGateway, "Failed to reach D&D Beyond.");
            }

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new DndBeyondRequestException(response.StatusCode, "D&D Beyond bearer token is invalid or has expired.");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                TimeSpan? retryAfter = null;
                if (response.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable)
                {
                    retryAfter = ParseRetryAfter(response.Headers);
                }

                throw new DndBeyondRequestException(
                    response.StatusCode,
                    $"D&D Beyond request failed with status {(int)response.StatusCode}.",
                    retryAfter);
            }

            return JsonDocument.Parse(string.IsNullOrWhiteSpace(response.Content) ? "{}" : response.Content);
        }

        private static bool IsAuthFailure(DndBeyondRequestException exception)
            => exception.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

        private static string? ParseBearerToken(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("token", out var token) && token.ValueKind == JsonValueKind.String)
                {
                    return token.GetString();
                }
            }
            catch (JsonException)
            {
                // Fall through and treat the response as malformed.
            }

            return null;
        }

        private static TimeSpan? ParseRetryAfter(IEnumerable<HeaderParameter>? headers)
        {
            if (headers is null)
            {
                return null;
            }

            var value = headers
                .Where(h => string.Equals(h.Name, "Retry-After", StringComparison.OrdinalIgnoreCase))
                .Select(h => h.Value?.ToString())
                .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

            if (value is null)
            {
                return null;
            }

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

            return null;
        }

        private sealed record CacheEntry(string Token, DateTimeOffset ExpiresAt);
    }
}
