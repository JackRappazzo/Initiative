using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickStatBlockBuilderTests.BuildTests
{
    public class GivenExpertSidekick : WhenTestingBuild
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(SidekickIsExpertLevel3)
            .And(BaseRawDataIsSet)
            .When(BuildIsCalled)
            .Then(ShouldOverrideCoreStats)
            .And(ShouldRemoveChallengeRating)
            .And(ShouldRecomputeSkills)
            .And(ShouldAppendClassFeatures);

        [Given]
        public void NameIsSet()
        {
            Name = "Sir Fluffy";
        }

        [Given]
        public void SidekickIsExpertLevel3()
        {
            Sidekick = CreateSidekick(SidekickClass.Expert, 3);
        }

        [Given]
        public void BaseRawDataIsSet()
        {
            BaseRawData = CreateWolfRawData();
        }

        [Then]
        public void ShouldOverrideCoreStats()
        {
            Assert.That(Result["name"].AsString, Is.EqualTo("Sir Fluffy"));
            Assert.That(Result["hp"].AsBsonDocument["average"].AsInt32, Is.EqualTo(21));
            Assert.That(Result["hp"].AsBsonDocument["formula"].AsString, Is.EqualTo("3d8 + 3"));
            Assert.That(Result["pb"].AsInt32, Is.EqualTo(2));
            Assert.That(Result["dex"].AsInt32, Is.EqualTo(17));
        }

        [Then]
        public void ShouldRemoveChallengeRating()
        {
            Assert.That(Result["cr"], Is.TypeOf<BsonNull>());
        }

        [Then]
        public void ShouldRecomputeSkills()
        {
            Assert.That(Result["skill"].AsBsonDocument["stealth"].AsString, Is.EqualTo("+5"));
            Assert.That(Result["skill"].AsBsonDocument["perception"].AsString, Is.EqualTo("+3"));
        }

        [Then]
        public void ShouldAppendClassFeatures()
        {
            var traits = Result["trait"].AsBsonArray;
            Assert.That(traits.Count, Is.EqualTo(5));
            Assert.That(traits[0].AsBsonDocument["name"].AsString, Is.EqualTo("Keen Hearing and Smell"));
            Assert.That(traits[1].AsBsonDocument["name"].AsString, Is.EqualTo("Bonus Proficiencies"));
            Assert.That(traits[4].AsBsonDocument["name"].AsString, Is.EqualTo("Expertise"));
        }
    }
}
