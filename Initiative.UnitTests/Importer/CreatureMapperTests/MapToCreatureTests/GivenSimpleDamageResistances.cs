using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenSimpleDamageResistances : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasSimpleDamageResistances)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapDamageResistances);

        [Given]
        public void MonsterJsonHasSimpleDamageResistances()
        {
            var resistanceElements = new List<JsonElement>
            {
                JsonDocument.Parse("\"fire\"").RootElement,
                JsonDocument.Parse("\"cold\"").RootElement,
                JsonDocument.Parse("\"bludgeoning\"").RootElement
            };

            MonsterJson = new MonsterJson
            {
                Name = "Fire Elemental",
                DamageResistances = resistanceElements,
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 102, Formula = "12d10+36" },
                Dexterity = 17
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldMapDamageResistances()
        {
            // Extract the resistances using the public method
            var resistances = CreatureMapper.ExtractDamageResistancesPublic(MonsterJson.DamageResistances);
            
            Assert.That(resistances, Is.Not.Null);
            Assert.That(resistances, Has.Count.EqualTo(3));
            
            var fireResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Fire);
            Assert.That(fireResistance, Is.Not.Null);
            Assert.That(fireResistance.RawValue, Is.EqualTo("fire"));
            Assert.That(fireResistance.GetDisplayName(), Is.EqualTo("Fire"));
            
            var coldResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Cold);
            Assert.That(coldResistance, Is.Not.Null);
            Assert.That(coldResistance.RawValue, Is.EqualTo("cold"));
            
            var bludgeoningResistance = resistances.FirstOrDefault(r => r.DamageType == DamageType.Bludgeoning);
            Assert.That(bludgeoningResistance, Is.Not.Null);
            Assert.That(bludgeoningResistance.RawValue, Is.EqualTo("bludgeoning"));
        }
    }
}