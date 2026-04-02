namespace Initiative.Api.Messages.Spell
{
    public class SearchSpellsRequest
    {
        public IEnumerable<string>? SpellSourceIds { get; set; }
        public string? Name { get; set; }
        public string? School { get; set; }
        public int PageSize { get; set; } = 20;
        public int Skip { get; set; } = 0;
    }
}
