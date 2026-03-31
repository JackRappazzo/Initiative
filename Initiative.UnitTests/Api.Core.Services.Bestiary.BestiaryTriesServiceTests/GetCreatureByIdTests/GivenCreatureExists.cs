using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetCreatureByIdTests
{
    public class GivenCreatureExists : WhenTestingGetCreatureById
    {
        private string _bestiaryId;
        private BestiaryCreatureDocument _creature;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CreatureIdIsSet)
            .And(RepositoryReturnsCreature)
            .When(GetCreatureByIdIsCalled)
            .Then(ShouldReturnCreature)
            .And(ShouldForwardIdToRepository);

        [Given]
        public void CreatureIdIsSet()
        {
            _bestiaryId = NewId();
            CreatureId = NewId();
        }

        [Given]
        public void RepositoryReturnsCreature()
        {
            _creature = new BestiaryCreatureDocument
            {
                Id = CreatureId,
                BestiaryId = _bestiaryId,
                Name = "Goblin",
                Source = "MM",
                CreatureType = "humanoid",
                ChallengeRating = "1/4",
                IsLegendary = false,
                RawData = new BsonDocument()
            };

            BestiaryRepository.GetCreatureById(CreatureId, CancellationToken)
                .Returns(_creature);
        }

        [Then]
        public void ShouldReturnCreature()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result!.Id, Is.EqualTo(CreatureId));
            Assert.That(Result.Name, Is.EqualTo("Goblin"));
        }

        [Then]
        public void ShouldForwardIdToRepository()
        {
            BestiaryRepository.Received(1).GetCreatureById(CreatureId, CancellationToken);
        }
    }
}
