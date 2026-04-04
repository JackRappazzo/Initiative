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

        public async Task SetEncounterCreatures(string encounterId, string ownerId, IEnumerable<EncounterCreature> creatures, CancellationToken cancellationToken)
        {
            if (await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken) == null)
            {
                throw new ArgumentException("Encounter not found.", nameof(encounterId));
            }

            await encounterRepository.SetEncounterCreatures(encounterId, creatures, cancellationToken);
        }

        public async Task RenameEncounter(string encounterId, string ownerId, string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Encounter name must not be empty.", nameof(name));
            }
            var encounter = await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken);

            if (encounter == null)
            {
                throw new ArgumentException("Encounter not found.", nameof(encounterId));
            }
            encounter.DisplayName = name;
            await encounterRepository.SetEncounterName(encounterId, name, cancellationToken);
        }

        public async Task<bool> DeleteEncounter(string encounterId, CancellationToken cancellationToken)
        {
            return await encounterRepository.DeleteEncounter(encounterId, cancellationToken);
        }

        public async Task SetEncounterTurnState(string encounterId, string ownerId, int turnIndex, int turnCount, CancellationToken cancellationToken)
        {
            if (await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken) == null)
            {
                throw new ArgumentException("Encounter not found.", nameof(encounterId));
            }
            await encounterRepository.SetEncounterTurnState(encounterId, turnIndex, turnCount, cancellationToken);
        }

        public async Task SetViewersAllowed(string encounterId, string ownerId, bool viewersAllowed, CancellationToken cancellationToken)
        {
            if (await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken) == null)
            {
                throw new ArgumentException("Encounter not found.", nameof(encounterId));
            }
            await encounterRepository.SetEncounterViewersAllowed(encounterId, viewersAllowed, cancellationToken);
        }

        public async Task SetEncounterPartyLevels(string encounterId, string ownerId, IEnumerable<int> partyLevels, CancellationToken cancellationToken)
        {
            if (await encounterRepository.FetchEncounterById(encounterId, ownerId, cancellationToken) == null)
            {
                throw new ArgumentException("Encounter not found.", nameof(encounterId));
            }

            var sanitizedLevels = (partyLevels ?? [])
                .Select(level => Math.Clamp(level, 1, 20))
                .ToList();

            await encounterRepository.SetEncounterPartyLevels(encounterId, sanitizedLevels, cancellationToken);
        }
    }
}
