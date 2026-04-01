using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.PartyRepositoryTests.FetchPartyTests
{
    public class GivenPartyExists : WhenTestingFetchParty
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(PartyExistsInDatabase)
            .When(FetchPartyIsCalled)
            .Then(ShouldReturnParty)
            .And(ShouldHaveCorrectName)
            .And(ShouldHaveMembers);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public async Task PartyExistsInDatabase()
        {
            var members = new List<PartyMember>
            {
                new PartyMember { Name = "Legolas", Level = 7 }
            };
            PartyId = await PartyRepository.CreateParty(OwnerId, "Fellowship", members, CancellationToken);
        }

        [Then]
        public void ShouldReturnParty()
        {
            Assert.That(Result, Is.Not.Null);
        }

        [Then]
        public void ShouldHaveCorrectName()
        {
            Assert.That(Result!.Name, Is.EqualTo("Fellowship"));
        }

        [Then]
        public void ShouldHaveMembers()
        {
            Assert.That(Result!.Members.Count(), Is.EqualTo(1));
            Assert.That(Result!.Members.First().Name, Is.EqualTo("Legolas"));
            Assert.That(Result!.Members.First().Level, Is.EqualTo(7));
        }
    }
}
