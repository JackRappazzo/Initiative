namespace Initiative.Api.Messages.Spell
{
    public class SearchSpellsResponse
    {
        public required IEnumerable<SpellItem> Spells { get; set; }
        public long TotalCount { get; set; }

        public class SpellItem
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public required string SpellSourceId { get; set; }
            public string? Source { get; set; }
            public string? School { get; set; }
        }
    }
}
