using Initiative.Persistence.Models.Spell;

namespace Import.Bestiaries.Core.Parsing
{
    public interface ISpellParser
    {
        List<SpellDocument> Parse(string spellSourceId, string source, Stream jsonStream);
    }
}
