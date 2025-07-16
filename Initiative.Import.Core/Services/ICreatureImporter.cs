using Initiative.Persistence.Models.Encounters;

namespace Initiative.Import.Core.Services
{
    public interface ICreatureImporter
    {
        Task<List<Creature>> ImportFromFile(string filePath, CancellationToken cancellationToken = default);
        List<Creature> ImportFromJson(string jsonContent);
        Task<List<Creature>> ImportFromStream(Stream stream, CancellationToken cancellationToken = default);
    }
}