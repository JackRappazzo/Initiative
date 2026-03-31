using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class SearchCreaturesResult
    {
        public required IEnumerable<BestiaryCreatureDocument> Creatures { get; set; }
        public long TotalCount { get; set; }
    }
}
