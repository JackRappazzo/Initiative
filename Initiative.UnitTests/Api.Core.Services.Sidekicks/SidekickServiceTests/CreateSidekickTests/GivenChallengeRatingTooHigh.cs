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
    public class GivenChallengeRatingTooHigh : WhenTestingCreateSidekick
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(PartyAndOwnerAreSet)
            .And(PartyExists)
            .And(BaseCreatureHasCr1)
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
        public void BaseCreatureHasCr1()
        {
            BestiaryRepository.GetCreatureById(Data.BaseCreatureId, CancellationToken)
                .Returns(new BestiaryCreatureDocument
                {
                    BestiaryId = ObjectId.GenerateNewId().ToString(),
                    Name = "Dire Wolf",
                    ChallengeRating = "1",
                    IsLegendary = false,
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
