using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using MongoDB.Bson;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.DeleteServiceTests
{
    public abstract class WhenTestingDeleteEncounter : WhenTestingEncounterService
    {
        protected bool Result;
        protected string EncounterId;



        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = ObjectId.GenerateNewId().ToString();
        }

        [When]
        public async Task DeleteIsCalled()
        {
            Result = await EncounterService.DeleteEncounter(EncounterId, CancellationToken);
        }
    }
}
