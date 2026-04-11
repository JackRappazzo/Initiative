using Initiative.Persistence.Models.Bestiary;
using Initiative.Api.Core.Services.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.UpdateCustomCreatureTests
{
    public class GivenBestiaryOwnedByUserAndCreatureExists : WhenTestingUpdateCustomCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputsAreSet)
            .And(BestiaryExistsAndIsOwned)
            .And(CreatureExists)
            .And(RawDataBuilderReturnsRawData)
            .When(UpdateCustomCreatureIsCalled)
            .Then(ShouldUpdateCreatureWithBuiltRawData)
            .And(ShouldUseRawDataBuilder);

        [Given]
        public void InputsAreSet()
        {
            CreatureId = NewId();
            BestiaryId = NewId();
            UserId = NewId();
            Data = new CustomCreatureData
            {
                Name = "Veteran",
                CreatureType = "humanoid",
                ChallengeRating = "3",
                IsLegendary = true
            };
            RawData = new BsonDocument { { "name", Data.Name }, { "marker", "updated" } };
        }

        [Given]
        public void BestiaryExistsAndIsOwned()
        {
            BestiaryRepository.GetBestiaryById(BestiaryId, CancellationToken)
                .Returns(new BestiaryDocument { Id = BestiaryId, OwnerId = UserId, Name = "Custom" });
        }

        [Given]
        public void CreatureExists()
        {
            ExistingCreature = new BestiaryCreatureDocument
            {
                Id = CreatureId,
                BestiaryId = BestiaryId,
                Name = "Old Name",
                CreatureType = "old-type",
                ChallengeRating = "1/2",
                IsLegendary = false,
                RawData = new BsonDocument()
            };

            BestiaryRepository.GetCreatureById(CreatureId, CancellationToken)
                .Returns(ExistingCreature);
        }

        [Given]
        public void RawDataBuilderReturnsRawData()
        {
            CustomCreatureRawDataBuilder.Build(Data).Returns(RawData);
        }

        [Then]
        public void ShouldUpdateCreatureWithBuiltRawData()
        {
            BestiaryRepository.Received(1).UpdateCreature(
                Arg.Is<BestiaryCreatureDocument>(c =>
                    c.Id == CreatureId &&
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
