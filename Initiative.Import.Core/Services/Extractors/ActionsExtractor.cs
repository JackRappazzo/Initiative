using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

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
                Descriptions = a.Entries.Select(ConvertEntryToString).ToArray()
            });
        }

        /// <summary>
        /// Converts a JsonElement entry to a readable string
        /// </summary>
        /// <param name="entry">JsonElement that can be a string or complex object</param>
        /// <returns>Readable string representation</returns>
        private string ConvertEntryToString(JsonElement entry)
        {
            if (entry.ValueKind == JsonValueKind.String)
            {
                return entry.GetString() ?? string.Empty;
            }
            
            if (entry.ValueKind == JsonValueKind.Object)
            {
                // Handle list objects
                if (entry.TryGetProperty("type", out var typeProperty) && 
                    typeProperty.GetString() == "list" &&
                    entry.TryGetProperty("items", out var itemsProperty) &&
                    itemsProperty.ValueKind == JsonValueKind.Array)
                {
                    var listItems = new List<string>();
                    
                    foreach (var item in itemsProperty.EnumerateArray())
                    {
                        if (item.TryGetProperty("name", out var nameProperty) &&
                            item.TryGetProperty("entries", out var entriesProperty) &&
                            entriesProperty.ValueKind == JsonValueKind.Array)
                        {
                            var itemName = nameProperty.GetString() ?? "";
                            var itemEntries = entriesProperty.EnumerateArray()
                                .Where(e => e.ValueKind == JsonValueKind.String)
                                .Select(e => e.GetString() ?? "")
                                .Where(s => !string.IsNullOrEmpty(s));
                            
                            listItems.Add($"{itemName}: {string.Join(" ", itemEntries)}");
                        }
                    }
                    
                    return string.Join("; ", listItems);
                }
                
                // For other object types, try to extract meaningful text
                return ExtractTextFromObject(entry);
            }
            
            return entry.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Extracts readable text from complex JSON objects
        /// </summary>
        /// <param name="obj">JsonElement object</param>
        /// <returns>Readable string representation</returns>
        private string ExtractTextFromObject(JsonElement obj)
        {
            // Try to find text content in common properties
            var textProperties = new[] { "text", "entry", "entries", "name" };
            
            foreach (var prop in textProperties)
            {
                if (obj.TryGetProperty(prop, out var property))
                {
                    if (property.ValueKind == JsonValueKind.String)
                    {
                        return property.GetString() ?? string.Empty;
                    }
                    else if (property.ValueKind == JsonValueKind.Array)
                    {
                        var texts = property.EnumerateArray()
                            .Where(e => e.ValueKind == JsonValueKind.String)
                            .Select(e => e.GetString() ?? "")
                            .Where(s => !string.IsNullOrEmpty(s));
                        
                        return string.Join(" ", texts);
                    }
                }
            }
            
            // Fallback: return raw JSON if no readable text found
            return obj.GetRawText();
        }
    }
}