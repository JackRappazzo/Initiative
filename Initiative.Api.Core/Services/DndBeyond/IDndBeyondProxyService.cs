using System.Net;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondCharacterListItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }

    public class DndBeyondCharacterDetail
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public string? ClassName { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int TemporaryHP { get; set; }
    }

    public class DndBeyondRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public TimeSpan? RetryAfter { get; }

        public DndBeyondRequestException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public DndBeyondRequestException(HttpStatusCode statusCode, string message, TimeSpan? retryAfter) : base(message)
        {
            StatusCode = statusCode;
            RetryAfter = retryAfter;
        }
    }

    public interface IDndBeyondProxyService
    {
        Task<IEnumerable<DndBeyondCharacterListItem>> GetCharacters(string token, CancellationToken cancellationToken);

        Task<DndBeyondCharacterDetail?> GetCharacter(string token, string characterId, CancellationToken cancellationToken);
    }
}
