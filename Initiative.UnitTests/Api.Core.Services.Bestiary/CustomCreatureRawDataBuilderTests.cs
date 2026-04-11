using Initiative.Api.Core.Services.Bestiary;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary
{
    public class CustomCreatureRawDataBuilderTests
    {
        [Test]
        public void Build_WithCoreFields_MapsExpectedDocumentShape()
        {
            var sut = new CustomCreatureRawDataBuilder();
            var data = new CustomCreatureData
            {
                Name = "Harpy",
                Size = "M",
                CreatureType = "monstrosity",
                Subtype = "shapechanger",
                Alignment = "chaotic evil",
                ChallengeRating = "1",
                ProficiencyBonus = 2,
                HP = 38,
                HitDice = "7d8 + 7",
                AC = 13,
                AcNote = "natural armor",
                AbilityScores = new CustomCreatureAbilityScores
                {
                    Str = 12,
                    Dex = 13,
                    Con = 12,
                    Int = 7,
                    Wis = 10,
                    Cha = 13
                },
                Speed = new CustomCreatureSpeed
                {
                    Walk = 20,
                    Fly = 40,
                    CanHover = true
                },
                SavingThrows = new Dictionary<string, string> { ["wis"] = "+2" },
                Skills = new Dictionary<string, string> { ["perception"] = "+4" },
                DamageImmunities = new List<string> { "poison" },
                Senses = new List<string> { "passive Perception 14" },
                Languages = new List<string> { "Common" },
                Traits = "The harpy sings.",
                Actions = new List<CustomCreatureEntry>
                {
                    new() { Name = "Claw", Description = "Melee Weapon Attack" }
                },
                IsLegendary = false
            };

            var doc = sut.Build(data);

            Assert.That(doc["name"].AsString, Is.EqualTo("Harpy"));
            Assert.That(doc["size"].AsBsonArray[0].AsString, Is.EqualTo("M"));
            Assert.That(doc["type"].AsBsonDocument["type"].AsString, Is.EqualTo("monstrosity"));
            Assert.That(doc["type"].AsBsonDocument["tags"].AsBsonArray[0].AsString, Is.EqualTo("shapechanger"));
            Assert.That(doc["cr"].AsString, Is.EqualTo("1"));
            Assert.That(doc["pb"].AsInt32, Is.EqualTo(2));
            Assert.That(doc["hp"].AsBsonDocument["average"].AsInt32, Is.EqualTo(38));
            Assert.That(doc["ac"].AsBsonArray[0].AsBsonDocument["ac"].AsInt32, Is.EqualTo(13));
            Assert.That(doc["str"].AsInt32, Is.EqualTo(12));
            Assert.That(doc["speed"].AsBsonDocument["fly"].AsBsonDocument["number"].AsInt32, Is.EqualTo(40));
            Assert.That(doc["save"].AsBsonDocument["wis"].AsString, Is.EqualTo("+2"));
            Assert.That(doc["skill"].AsBsonDocument["perception"].AsString, Is.EqualTo("+4"));
            Assert.That(doc["immune"].AsBsonArray[0].AsString, Is.EqualTo("poison"));
            Assert.That(doc["trait"].AsBsonArray[0].AsBsonDocument["name"].AsString, Is.EqualTo("Traits"));
            Assert.That(doc["action"].AsBsonArray[0].AsBsonDocument["name"].AsString, Is.EqualTo("Claw"));
        }

        [Test]
        public void Build_WithSpellcastingFreeform_BuildsHeaderEntriesAndOmitsStructuredSections()
        {
            var sut = new CustomCreatureRawDataBuilder();
            var data = new CustomCreatureData
            {
                Name = "Archmage",
                Spellcasting = new CustomCreatureSpellcasting
                {
                    Ability = "int",
                    SpellSaveDc = 17,
                    SpellAttackBonus = 9,
                    Description = "The archmage is an 18th-level spellcaster.",
                    FreeformText = "At will: mage hand\n3/day each: fly, fireball",
                    SlotSpells = new Dictionary<string, List<string>>
                    {
                        ["0"] = new List<string> { "mage hand" },
                        ["1"] = new List<string> { "shield" }
                    },
                    DailySpells = new List<string> { "3/day: fireball" }
                }
            };

            var doc = sut.Build(data);
            var spellcasting = doc["spellcasting"].AsBsonArray[0].AsBsonDocument;
            var headerEntries = spellcasting["headerEntries"].AsBsonArray;

            Assert.That(spellcasting["ability"].AsString, Is.EqualTo("int"));
            Assert.That(headerEntries[0].AsString, Is.EqualTo("The archmage is an 18th-level spellcaster."));
            Assert.That(headerEntries[1].AsString, Is.EqualTo("spell save DC 17, +9 to spell attack rolls"));
            Assert.That(headerEntries[2].AsString, Is.EqualTo("At will: mage hand"));
            Assert.That(headerEntries[3].AsString, Is.EqualTo("3/day each: fly, fireball"));
            Assert.That(spellcasting.Contains("spells"), Is.False);
            Assert.That(spellcasting.Contains("daily"), Is.False);
            Assert.That(spellcasting.Contains("will"), Is.False);
        }
    }
}
