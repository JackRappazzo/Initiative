using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenSpecialDamageResistances : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasSpecialDamageResistances)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapSpecialDamageResistances);

        [Given]
        public void MonsterJsonHasSpecialDamageResistances()
        {
            // Create a special resistance object like the one in the JSON example
            var specialResistanceJson = """
            {
                "special": "Damage type chosen for the Draconic Origin trait below"
            }
            """;
            
            var resistanceElements = new List<JsonElement>
            {
                JsonDocument.Parse("\"fire\"").RootElement,
                JsonDocument.Parse(specialResistanceJson).RootElement
            };

            MonsterJson = new MonsterJson
            {
                Name = "Dragonborn Sorcerer",
                DamageResistances = resistanceElements,
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 68, Formula = "8d8+24" },
                Dexterity = 14
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldMapSpecialDamageResistances()
        {
            // Extract the resistances using the public method
            var resistances = CreatureMapper.ExtractDamageResistancesPublic(MonsterJson.DamageResistances);
            
            Assert.That(resistances, Is.Not.Null);
            Assert.That(resistances, Has.Count.EqualTo(2));
            
            // Check the regular fire resistance
            var fireResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Fire);
            Assert.That(fireResistance, Is.Not.Null);
            Assert.That(fireResistance.RawValue, Is.EqualTo("fire"));
            Assert.That(fireResistance.Special, Is.Null);
            
            // Check the special resistance
            var specialResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Special);
            Assert.That(specialResistance, Is.Not.Null);
            Assert.That(specialResistance.Special, Is.EqualTo("Damage type chosen for the Draconic Origin trait below"));
            Assert.That(specialResistance.RawValue, Is.EqualTo("Damage type chosen for the Draconic Origin trait below"));
            Assert.That(specialResistance.GetDisplayName(), Is.EqualTo("Damage type chosen for the Draconic Origin trait below"));
        }
    }
}