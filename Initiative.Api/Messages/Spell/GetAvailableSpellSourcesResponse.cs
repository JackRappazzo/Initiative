namespace Initiative.Api.Messages.Spell
{
    public class GetAvailableSpellSourcesResponse
    {
        public required IEnumerable<SpellSourceItem> SpellSources { get; set; }

        public class SpellSourceItem
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public string? Source { get; set; }
        }
    }
}
