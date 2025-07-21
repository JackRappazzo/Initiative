using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract actions from JSON data
    /// </summary>
    public class ActionsExtractor : IActionsExtractor
    {
        /// <summary>
        /// Extracts actions from a list of ActionJson objects
        /// </summary>
        /// <param name="actions">List of ActionJson objects</param>
        /// <returns>Enumerable of CreatureAction objects</returns>
        public IEnumerable<CreatureAction> ExtractActions(List<ActionJson>? actions)
        {
            if (actions == null || actions.Count == 0)
                return Enumerable.Empty<CreatureAction>();
            
            return actions.Select(a => new CreatureAction()
            {
                Name = a.Name,
                Descriptions = a.Entries.ToArray()
            });
        }
    }
}