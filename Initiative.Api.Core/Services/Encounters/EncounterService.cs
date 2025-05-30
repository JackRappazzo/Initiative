using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Core.Services.Encounters
{
    public class EncounterService : IEncounterService
    {
        protected readonly IEncounterRepository encounterRepository;

        public EncounterService(IEncounterRepository encounterRepository)
        {
            this.encounterRepository = encounterRepository;
        }

        public async Task<string> CreateEncounter(string ownerId, string encounterName, CancellationToken cancellationToken)
        {
            var result = await encounterRepository.CreateEncounter(ownerId, encounterName, cancellationToken);

            return result;
        }

        public async Task<IEnumerable<EncounterListItem>> GetEncounterList(string ownerId, CancellationToken cancellationToken)
        {
            var result = await encounterRepository.FetchEncounterListByUserId(ownerId, cancellationToken);

            return result?.Select(r => new EncounterListItem
            {
                EncounterId = r.EncounterId,
                EncounterName = r.EncounterName,
                NumberOfCreatures = r.NumberOfCreatures,
                CreatedAt = r.CreatedAt
            }) ?? new List<EncounterListItem>();
        }

        public async Task<Encounter> GetEncounter(string encounterId, string ownerId, CancellationToken cancellationToken)
        {
            var result = await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken);
            return result;
        }
    }
}
