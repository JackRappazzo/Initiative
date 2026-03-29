using Initiative.Persistence.Models.Bestiary;

namespace Import.Bestiaries.Core.Parsing
{
    public interface IFivEToolsParser
    {
        List<BestiaryCreatureDocument> Parse(string bestiaryId, string source, Stream jsonStream);
    }
}
