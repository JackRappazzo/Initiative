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
    public class GivenValidBaseCreature : WhenTestingCreateSidekick
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(PartyAndOwnerAreSet)
            .And(PartyExists)
            .And(BaseCreatureIsValid)
            .And(BuilderReturnsRawData)
            .When(CreateSidekickIsCalled)
            .Then(ShouldReturnSidekickMember)
            .And(ShouldUpdatePartyMembers);

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
                Level = 3,
                AbilityScores = new SidekickAbilityScores { Constitution = 12 }
            };
        }

        [Given]
        public void PartyExists()
        {
            PartyRepository.FetchPartyById(PartyId, OwnerId, CancellationToken)
                .Returns(new PartyDocument { Name = "Test Party", OwnerId = OwnerId, Members = new List<PartyMember>() });
            PartyRepository.UpdatePartyMembers(PartyId, OwnerId, Arg.Any<IEnumerable<PartyMember>>(), CancellationToken)
                .Returns(true);
        }

        [Given]
        public void BaseCreatureIsValid()
        {
            BestiaryRepository.GetCreatureById(Data.BaseCreatureId, CancellationToken)
                .Returns(new BestiaryCreatureDocument
                {
                    BestiaryId = ObjectId.GenerateNewId().ToString(),
                    Name = "Wolf",
                    ChallengeRating = "1/4",
                    IsLegendary = false,
                    RawData = new BsonDocument()
                });
        }

        [Given]
        public void BuilderReturnsRawData()
        {
            StatBlockBuilder.Build(Name, Arg.Any<Sidekick>(), Arg.Any<BsonDocument>())
                .Returns(new BsonDocument());
        }

        [Then]
        public void ShouldReturnSidekickMember()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Name, Is.EqualTo("Sir Fluffy"));
            Assert.That(Result.Sidekick, Is.Not.Null);
            Assert.That(Result.Sidekick!.Level, Is.EqualTo(3));
        }

        [Then]
        public void ShouldUpdatePartyMembers()
        {
            PartyRepository.Received(1)
                .UpdatePartyMembers(PartyId, OwnerId, Arg.Any<IEnumerable<PartyMember>>(), CancellationToken);
        }
    }
}
