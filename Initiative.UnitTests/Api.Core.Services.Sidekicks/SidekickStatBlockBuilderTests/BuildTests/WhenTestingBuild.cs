using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickStatBlockBuilderTests.BuildTests
{
    public abstract class WhenTestingBuild : WhenTestingSidekickStatBlockBuilder
    {
        protected string Name;
        protected Sidekick Sidekick;
        protected BsonDocument BaseRawData;
        protected BsonDocument Result;

        [When]
        public void BuildIsCalled()
        {
            Result = Builder.Build(Name, Sidekick, BaseRawData);
        }

        protected static BsonDocument CreateWolfRawData() => new()
        {
            { "name", "Wolf" },
            { "size", new BsonArray { "M" } },
            { "type", "beast" },
            { "alignment", new BsonArray { "U" } },
            { "ac", new BsonArray { 13 } },
            { "hp", new BsonDocument { { "average", 11 }, { "formula", "2d8+2" } } },
            { "speed", new BsonDocument { { "walk", 40 } } },
            { "str", 12 }, { "dex", 15 }, { "con", 12 }, { "int", 3 }, { "wis", 12 }, { "cha", 6 },
            { "skill", new BsonDocument { { "perception", "+3" }, { "stealth", "+4" } } },
            { "cr", "1/4" },
            { "trait", new BsonArray { new BsonDocument { { "name", "Keen Hearing and Smell" }, { "entries", new BsonArray { "advantage on Perception" } } } } }
        };

        protected static Sidekick CreateSidekick(SidekickClass sidekickClass, int level, SidekickSpellcasting? spellcasting = null)
            => new()
            {
                BaseCreatureId = ObjectId.GenerateNewId().ToString(),
                BaseCreatureName = "Wolf",
                Class = sidekickClass,
                Level = level,
                AbilityScores = new SidekickAbilityScores
                {
                    Strength = 12,
                    Dexterity = 17,
                    Constitution = 12,
                    Intelligence = 3,
                    Wisdom = 12,
                    Charisma = 6
                },
                MaxHitPoints = 21,
                HitDiceCount = level,
                HitDieSize = SidekickRules.HitDieSize(sidekickClass),
                HpRolls = new List<SidekickHpRoll>(),
                Spellcasting = spellcasting,
                Choices = new List<SidekickChoice>(),
                RawData = new BsonDocument()
            };
    }
}
