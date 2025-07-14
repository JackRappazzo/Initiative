using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenEmptyArmorClassList : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasEmptyArmorClassList)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldUseDefaultArmorClass);

        [Given]
        public void MonsterJsonHasEmptyArmorClassList()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Unarmored Creature",
                ArmorClass = new List<System.Text.Json.JsonElement>(), // Empty list
                HitPoints = new HitPointsJson { Average = 10, Formula = "2d8+2" },
                Dexterity = 12,
                Initiative = null
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldUseDefaultArmorClass()
        {
            Assert.That(Result.ArmorClass, Is.EqualTo(10)); // Default AC when list is empty
        }
    }
}