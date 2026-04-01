using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.PartyRepositoryTests.CreatePartyTests
{
    public class GivenHappyPath : WhenTestingCreateParty
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(NameIsSet)
            .And(MembersAreSet)
            .When(CreatePartyIsCalled)
            .Then(ShouldReturnId)
            .And(ShouldStoreParty);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void NameIsSet()
        {
            Name = "Test Party";
        }

        [Given]
        public void MembersAreSet()
        {
            Members =
            [
                new PartyMember { Name = "Aragorn", Level = 5 },
                new PartyMember { Name = "Gandalf", Level = 10 }
            ];
        }

        [Then]
        public void ShouldReturnId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreParty()
        {
            var party = await PartyRepository.FetchPartyById(Result, OwnerId, CancellationToken);

            Assert.That(party, Is.Not.Null);
            Assert.That(party.Name, Is.EqualTo(Name));
            Assert.That(party.Members.Count(), Is.EqualTo(2));
        }
    }
}
