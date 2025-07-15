using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenMixedDamageResistances : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasMixedDamageResistances)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapAllDamageResistances);

        [Given]
        public void MonsterJsonHasMixedDamageResistances()
        {
            // Create a mix of simple strings and special objects
            var specialResistanceJson = """
            {
                "special": "Damage type chosen for the Draconic Origin trait below"
            }
            """;
            
            var resistanceElements = new List<JsonElement>
            {
                JsonDocument.Parse("\"fire\"").RootElement,
                JsonDocument.Parse("\"cold\"").RootElement,
                JsonDocument.Parse(specialResistanceJson).RootElement,
                JsonDocument.Parse("\"lightning\"").RootElement
            };

            MonsterJson = new MonsterJson
            {
                Name = "Mixed Resistance Creature",
                DamageResistances = resistanceElements,
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("16").RootElement },
                HitPoints = new HitPointsJson { Average = 85, Formula = "10d8+30" },
                Dexterity = 15
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldMapAllDamageResistances()
        {
            // Extract the resistances using the public method
            var resistances = CreatureMapper.ExtractDamageResistancesPublic(MonsterJson.DamageResistances);
            
            Assert.That(resistances, Is.Not.Null);
            Assert.That(resistances, Has.Count.EqualTo(4));
            
            // Check simple resistances
            var fireResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Fire);
            Assert.That(fireResistance, Is.Not.Null);
            Assert.That(fireResistance.Special, Is.Null);
            
            var coldResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Cold);
            Assert.That(coldResistance, Is.Not.Null);
            Assert.That(coldResistance.Special, Is.Null);
            
            var lightningResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Lightning);
            Assert.That(lightningResistance, Is.Not.Null);
            Assert.That(lightningResistance.Special, Is.Null);
            
            // Check special resistance
            var specialResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Special);
            Assert.That(specialResistance, Is.Not.Null);
            Assert.That(specialResistance.Special, Is.Not.Null.And.Not.Empty);
            Assert.That(specialResistance.Special, Is.EqualTo("Damage type chosen for the Draconic Origin trait below"));
        }
    }
}