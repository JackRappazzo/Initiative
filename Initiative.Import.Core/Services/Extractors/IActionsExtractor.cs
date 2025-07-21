using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting actions from JSON data
    /// </summary>
    public interface IActionsExtractor
    {
        /// <summary>
        /// Extracts actions from a list of ActionJson objects
        /// </summary>
        /// <param name="actions">List of ActionJson objects</param>
        /// <returns>Enumerable of CreatureAction objects</returns>
        IEnumerable<CreatureAction> ExtractActions(List<ActionJson>? actions);
    }
}