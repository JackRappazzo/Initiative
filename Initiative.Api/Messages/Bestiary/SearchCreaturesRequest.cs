using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Api.Messages.Bestiary
{
    public class SearchCreaturesRequest
    {
        public IEnumerable<string>? BestiaryIds { get; set; }
        public string? Name { get; set; }
        public string? CreatureType { get; set; }
        public string? ChallengeRating { get; set; }
        public bool? IsLegendary { get; set; }
        public CreatureSortBy SortBy { get; set; } = CreatureSortBy.Name;
        public bool SortDescending { get; set; } = false;
        public int PageSize { get; set; } = 20;
        public int Skip { get; set; } = 0;
    }
}
