using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.FetchEncounterTests
{
    public class GivenEncounterDoesNotExist : WhenTestingFetchEncounter
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(EncounterIdIsSet)
            .And(EncounterDoesNotExist)
            .When(FetchEncounterIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void EncounterDoesNotExist()
        {
            //Do not add an encounter to fetch
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null, "Expected Result to be null when the encounter does not exist.");
        }
    }
}
