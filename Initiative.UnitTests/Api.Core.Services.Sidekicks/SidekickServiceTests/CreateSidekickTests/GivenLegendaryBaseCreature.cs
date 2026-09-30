using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickServiceTests.CreateSidekickTests
{
    public class GivenLegendaryBaseCreature : WhenTestingCreateSidekick
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(PartyAndOwnerAreSet)
            .And(PartyExists)
            .And(BaseCreatureIsLegendary)
            .When(CreateSidekickIsCalled)
            .Then(ShouldThrowException);

        [Given]
        public void PartyAndOwnerAreSet()
        {
            PartyId = ObjectId.GenerateNewId().ToString();
            OwnerId = ObjectId.GenerateNewId().ToString();
            Name = "Sir Fluffy";
            Data = new SidekickData
            {
                BaseCreatureId = ObjectId.GenerateNewId().ToString(),
                Class = SidekickClass.Expert,
                Level = 1
            };
        }

        [Given]
        public void PartyExists()
        {
            PartyRepository.FetchPartyById(PartyId, OwnerId, CancellationToken)
                .Returns(new PartyDocument { Name = "Test Party", OwnerId = OwnerId, Members = new List<PartyMember>() });
        }

        [Given]
        public void BaseCreatureIsLegendary()
        {
            BestiaryRepository.GetCreatureById(Data.BaseCreatureId, CancellationToken)
                .Returns(new BestiaryCreatureDocument
                {
                    BestiaryId = ObjectId.GenerateNewId().ToString(),
                    Name = "Dragon Whelp",
                    ChallengeRating = "1/4",
                    IsLegendary = true,
                    RawData = new BsonDocument()
                });
        }

        [Then]
        public void ShouldThrowException()
        {
            Assert.That(ThrownException, Is.TypeOf<ArgumentException>());
        }
    }
}
