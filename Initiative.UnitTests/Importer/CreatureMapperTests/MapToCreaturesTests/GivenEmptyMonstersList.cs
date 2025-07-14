using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreaturesTests
{
    public class GivenEmptyMonstersList : WhenTestingMapToCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonstersListIsEmpty)
            .When(MapToCreaturesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyList);

        [Given]
        public void MonstersListIsEmpty()
        {
            Monsters = new List<MonsterJson>();
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyList()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Has.Count.EqualTo(0));
        }
    }
}