using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickStatBlockBuilderTests.BuildTests
{
    public class GivenSpellcasterSidekick : WhenTestingBuild
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(SidekickIsSpellcasterLevel3)
            .And(BaseRawDataIsSet)
            .When(BuildIsCalled)
            .Then(ShouldProduceSpellcastingEntry);

        [Given]
        public void NameIsSet()
        {
            Name = "Sir Fluffy";
        }

        [Given]
        public void SidekickIsSpellcasterLevel3()
        {
            var spellcasting = new SidekickSpellcasting
            {
                Kind = SidekickSpellcasterKind.Magician,
                Ability = "int",
                Cantrips = new List<string> { "fire bolt", "light" },
                Slots = SidekickRules.SpellSlots(3).ToDictionary(kv => kv.Key, kv => kv.Value),
                KnownSpells = new Dictionary<int, List<string>>
                {
                    [1] = new() { "magic missile", "shield" },
                    [2] = new() { "misty step" }
                }
            };

            Sidekick = CreateSidekick(SidekickClass.Spellcaster, 3, spellcasting);
        }

        [Given]
        public void BaseRawDataIsSet()
        {
            BaseRawData = CreateWolfRawData();
        }

        [Then]
        public void ShouldProduceSpellcastingEntry()
        {
            var sc = Result["spellcasting"].AsBsonArray[0].AsBsonDocument;
            Assert.That(sc["ability"].AsString, Is.EqualTo("int"));
            Assert.That(sc["will"].AsBsonArray.Count, Is.EqualTo(2));
            Assert.That(sc["will"].AsBsonArray[0].AsString, Is.EqualTo("fire bolt"));

            var spells = sc["spells"].AsBsonDocument;
            Assert.That(spells["1"].AsBsonDocument["slots"].AsInt32, Is.EqualTo(4));
            Assert.That(spells["1"].AsBsonDocument["spells"].AsBsonArray.Count, Is.EqualTo(2));
            Assert.That(spells["2"].AsBsonDocument["slots"].AsInt32, Is.EqualTo(2));
        }
    }
}
