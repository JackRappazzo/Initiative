using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.SearchCreaturesTests
{
    public class GivenMatchingCreaturesExist : WhenTestingSearchCreatures
    {
        private string _bestiaryId;
        private string _creatureId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(QueryIsSet)
            .And(RepositoryReturnsCreatures)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnCreatures)
            .And(ShouldForwardQueryToRepository);

        [Given]
        public void QueryIsSet()
        {
            _bestiaryId = NewId();
            _creatureId = NewId();
            Query = new BestiarySearchQuery
            {
                NameSearch = "Goblin",
                CreatureType = "humanoid",
                PageSize = 10,
                Skip = 0
            };
        }

        [Given]
        public void RepositoryReturnsCreatures()
        {
            BestiaryRepository.SearchCreatures(Query, CancellationToken)
                .Returns(new List<BestiaryCreatureDocument>
                {
                    new BestiaryCreatureDocument
                    {
                        Id = _creatureId,
                        BestiaryId = _bestiaryId,
                        Name = "Goblin",
                        Source = "XMM",
                        CreatureType = "humanoid",
                        ChallengeRating = "1/4",
                        IsLegendary = false,
                        RawData = new BsonDocument()
                    }
                });
        }

        [Then]
        public void ShouldReturnCreatures()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(1));
            Assert.That(Result.First().Name, Is.EqualTo("Goblin"));
        }

        [Then]
        public void ShouldForwardQueryToRepository()
        {
            BestiaryRepository.Received(1).SearchCreatures(Query, CancellationToken);
        }
    }
}
