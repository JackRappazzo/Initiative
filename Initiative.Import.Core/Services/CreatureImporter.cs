using Initiative.Import.Core.Models;
using Initiative.Import.Core.Services;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

namespace Initiative.Import.Core.Services
{
    /// <summary>
    /// Service for importing creatures from JSON files
    /// </summary>
    public class CreatureImporter
    {
        private readonly ICreatureMapper creatureMapper;

        public CreatureImporter(ICreatureMapper creatureMapper)
        {
            this.creatureMapper = creatureMapper;
        }

        /// <summary>
        /// Imports creatures from a JSON file path
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of imported creatures</returns>
        public async Task<List<Creature>> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            return ImportFromJson(jsonContent);
        }

        /// <summary>
        /// Imports creatures from JSON content
        /// </summary>
        /// <param name="jsonContent">The JSON content as a string</param>
        /// <returns>List of imported creatures</returns>
        public List<Creature> ImportFromJson(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentException("JSON content cannot be null or empty", nameof(jsonContent));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            try
            {
                var monsterManual = JsonSerializer.Deserialize<MonsterManualJson>(jsonContent, options);
                
                if (monsterManual?.Monsters == null || !monsterManual.Monsters.Any())
                {
                    return new List<Creature>();
                }

                return creatureMapper.MapToCreatures(monsterManual.Monsters);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse JSON content: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Imports creatures from a stream
        /// </summary>
        /// <param name="stream">Stream containing JSON data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of imported creatures</returns>
        public async Task<List<Creature>> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync(cancellationToken);
            return ImportFromJson(jsonContent);
        }
    }
}