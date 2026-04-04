using Initiative.Persistence.Models.Condition;

namespace Import.Bestiaries.Core.Parsing
{
    public interface IConditionParser
    {
        /// <summary>
        /// Parses the "condition" array from a 5etools conditions/diseases JSON stream
        /// into a list of <see cref="ConditionDocument"/> ready for upsert.
        /// </summary>
        List<ConditionDocument> Parse(string source, Stream jsonStream);
    }
}
