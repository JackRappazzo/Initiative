using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.PartyServiceTests.CreatePartyTests
{
    public class GivenHappyPath : WhenTestingCreateParty
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(NameIsSet)
            .And(MembersAreSet)
            .And(PartyRepositoryReturnsNewId)
            .When(CreatePartyIsCalled)
            .Then(ResultShouldNotBeNullOrEmpty)
            .And(ShouldReturnPartyId);

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
                new PartyMember { Name = "Fighter", Level = 3 }
            ];
        }

        [Given]
        public void PartyRepositoryReturnsNewId()
        {
            PartyRepository.CreateParty(OwnerId, Name, Members, CancellationToken)
                .Returns(ObjectId.GenerateNewId().ToString());
        }

        [Then]
        public void ShouldReturnPartyId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }
    }
}
