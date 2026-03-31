namespace Initiative.Api.Messages.Bestiary
{
    public class SearchCreaturesResponse
    {
        public required IEnumerable<CreatureItem> Creatures { get; set; }
        public long TotalCount { get; set; }

        public class CreatureItem
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public required string BestiaryId { get; set; }
            public string? Source { get; set; }
            public string? CreatureType { get; set; }
            public string? ChallengeRating { get; set; }
            public bool IsLegendary { get; set; }
        }
    }
}
