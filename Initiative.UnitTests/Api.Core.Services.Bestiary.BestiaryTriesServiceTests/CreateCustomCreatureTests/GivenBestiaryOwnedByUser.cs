using Initiative.Persistence.Models.Bestiary;
using Initiative.Api.Core.Services.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.CreateCustomCreatureTests
{
    public class GivenBestiaryOwnedByUser : WhenTestingCreateCustomCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputsAreSet)
            .And(BestiaryExistsAndIsOwned)
            .And(RawDataBuilderReturnsRawData)
            .When(CreateCustomCreatureIsCalled)
            .Then(ShouldReturnCreatureWithBuiltRawData)
            .And(ShouldPersistCreature)
            .And(ShouldUseRawDataBuilder);

        [Given]
        public void InputsAreSet()
        {
            BestiaryId = NewId();
            UserId = NewId();
            Data = new CustomCreatureData
            {
                Name = "Goblin Boss",
                CreatureType = "humanoid",
                ChallengeRating = "1",
                IsLegendary = false
            };
            RawData = new BsonDocument { { "name", Data.Name }, { "marker", "from-builder" } };
        }

        [Given]
        public void BestiaryExistsAndIsOwned()
        {
            BestiaryRepository.GetBestiaryById(BestiaryId, CancellationToken)
                .Returns(new BestiaryDocument { Id = BestiaryId, OwnerId = UserId, Name = "Custom" });

            BestiaryRepository.InsertCreature(Arg.Any<BestiaryCreatureDocument>(), CancellationToken)
                .Returns(NewId());
        }

        [Given]
        public void RawDataBuilderReturnsRawData()
        {
            CustomCreatureRawDataBuilder.Build(Data).Returns(RawData);
        }

        [Then]
        public void ShouldReturnCreatureWithBuiltRawData()
        {
            Assert.That(Result.BestiaryId, Is.EqualTo(BestiaryId));
            Assert.That(Result.Name, Is.EqualTo(Data.Name));
            Assert.That(Result.RawData, Is.SameAs(RawData));
        }

        [Then]
        public void ShouldPersistCreature()
        {
            BestiaryRepository.Received(1).InsertCreature(
                Arg.Is<BestiaryCreatureDocument>(c =>
                    c.BestiaryId == BestiaryId &&
                    c.Name == Data.Name &&
                    c.CreatureType == Data.CreatureType &&
                    c.ChallengeRating == Data.ChallengeRating &&
                    c.IsLegendary == Data.IsLegendary &&
                    ReferenceEquals(c.RawData, RawData)),
                CancellationToken);
        }

        [Then]
        public void ShouldUseRawDataBuilder()
        {
            CustomCreatureRawDataBuilder.Received(1).Build(Data);
        }
    }
}
