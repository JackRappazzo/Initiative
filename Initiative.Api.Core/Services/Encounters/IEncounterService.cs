﻿
namespace Initiative.Api.Core.Services.Encounters
{
    public interface IEncounterService
    {
        Task<string> CreateEncounter(string ownerId, string encounterName, CancellationToken cancellationToken);
        Task<IEnumerable<EncounterListItem>> GetEncounterList(string ownerId, CancellationToken cancellationToken);
    }
}