﻿using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Models.Encounters.Dtos;

namespace Initiative.Persistence.Repositories
{
    public interface IEncounterRepository
    {
        Task<string> CreateEncounter(string ownerId, string name, CancellationToken cancellationToken);
        Task<Encounter> FetchEncounterById(string encounterId, string ownerId, CancellationToken cancellationToken);
        Task<IEnumerable<EncounterListItemDto>> FetchEncounterListByUserId(string userId, CancellationToken cancellationToken);
    }
}