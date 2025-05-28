using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.FetchEncounterTests
{
    public abstract class WhenTestingFetchEncounter : WhenTestingEncounterRepository
    {

        protected string EncounterId;
        protected string UserId;
        protected Encounter Result;

        [When]
        public async Task FetchEncounterIsCalled()
        {
            Result = await EncounterRepository.FetchEncounterById(EncounterId, UserId, CancellationToken);
        }


        [Given]
        public void UserIdIsSet()
        {
            UserId = ObjectId.GenerateNewId().ToString();
        }
    }
}
