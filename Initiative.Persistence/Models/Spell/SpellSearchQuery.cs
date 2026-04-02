namespace Initiative.Persistence.Models.Spell
{
    public class SpellSearchQuery
    {
        /// <summary>
        /// Restrict results to spells belonging to one or more specific spell sources.
        /// Null or empty means search across all sources.
        /// </summary>
        public IEnumerable<string>? SpellSourceIds { get; set; }

        /// <summary>
        /// Prefix / type-ahead search on spell name. Case-insensitive.
        /// Null means no name filter.
        /// </summary>
        public string? NameSearch { get; set; }

        /// <summary>
        /// Filter by full English school name, e.g. "Evocation", "Necromancy".
        /// Case-insensitive. Null means no school filter.
        /// </summary>
        public string? School { get; set; }

        public int PageSize { get; set; } = 20;

        public int Skip { get; set; } = 0;
    }
}
