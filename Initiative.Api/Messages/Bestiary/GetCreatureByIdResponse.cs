namespace Initiative.Api.Messages.Bestiary
{
    public class GetCreatureByIdResponse
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string BestiaryId { get; set; }
        public string? Source { get; set; }

        /// <summary>
        /// The full 5etools creature JSON, serialised to a string and embedded as a raw
        /// JSON object so the frontend can parse it directly.
        /// </summary>
        public required object RawData { get; set; }
    }
}
