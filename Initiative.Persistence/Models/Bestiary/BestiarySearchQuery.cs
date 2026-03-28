namespace Initiative.Persistence.Models.Bestiary
{
    public class BestiarySearchQuery
    {
        /// <summary>
        /// Restrict results to creatures belonging to one or more specific bestiaries.
        /// Null or empty means search across all bestiaries.
        /// </summary>
        public IEnumerable<string>? BestiaryIds { get; set; }

        /// <summary>
        /// Prefix / type-ahead search on creature name. Case-insensitive.
        /// Null means no name filter.
        /// </summary>
        public string? NameSearch { get; set; }

        /// <summary>
        /// Filter by creature type string, e.g. "dragon", "humanoid".
        /// Case-insensitive. Null means no type filter.
        /// </summary>
        public string? CreatureType { get; set; }

        /// <summary>
        /// Filter by exact challenge rating string, e.g. "1/4", "5", "30".
        /// Null means no CR filter.
        /// </summary>
        public string? ChallengeRating { get; set; }

        /// <summary>
        /// When true, restrict to legendary creatures only.
        /// When false, restrict to non-legendary creatures only.
        /// Null means no legendary filter.
        /// </summary>
        public bool? IsLegendary { get; set; }

        /// <summary>Maximum number of results to return. Defaults to 20.</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Number of results to skip for pagination. Defaults to 0.</summary>
        public int Skip { get; set; } = 0;
    }
}
