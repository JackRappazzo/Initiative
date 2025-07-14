using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenNameWithSpecialCharacters : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasNameWithSpecialCharacters)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldGenerateCleanSystemName);

        [Given]
        public void MonsterJsonHasNameWithSpecialCharacters()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Tiamat's Dragon (Ancient Red)",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("22").RootElement },
                HitPoints = new HitPointsJson { Average = 546, Formula = "28d20+252" },
                Dexterity = 10,
                Initiative = null
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldGenerateCleanSystemName()
        {
            Assert.That(Result.SystemName, Is.EqualTo("tiamats-dragon-ancient-red"));
        }
    }
}