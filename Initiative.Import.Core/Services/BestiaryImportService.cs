using Initiative.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Import.Core.Services
{
    public class BestiaryImportService
    {
        private readonly ICreatureImporter creatureImporter;
        private readonly IBestiaryRepository bestiaryRepository;

        public BestiaryImportService(ICreatureImporter creatureImporter, IBestiaryRepository bestiaryRepository)
        {
            this.creatureImporter = creatureImporter;
            this.bestiaryRepository = bestiaryRepository;
        }

        /// <summary>
        /// Imports a system bestiary from a file and adds it to the repository.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bestiaryName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ImportSystemBestiary(string filePath, string bestiaryName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(bestiaryName))
            {
                throw new ArgumentException("Bestiary name cannot be null or empty", nameof(bestiaryName));
            }

            var creatures = await creatureImporter.ImportFromFile(filePath, cancellationToken);

            await bestiaryRepository.UpsertSystemBestiary(bestiaryName, creatures, cancellationToken);
           
        }
    }
}
