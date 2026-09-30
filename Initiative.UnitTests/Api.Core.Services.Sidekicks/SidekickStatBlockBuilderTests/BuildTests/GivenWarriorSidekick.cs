using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickStatBlockBuilderTests.BuildTests
{
    public class GivenWarriorSidekick : WhenTestingBuild
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(SidekickIsWarriorLevel5)
            .And(BaseRawDataIsSet)
            .When(BuildIsCalled)
            .Then(ShouldAddStrengthAndConstitutionSavingThrows);

        [Given]
        public void NameIsSet()
        {
            Name = "Sir Fluffy";
        }

        [Given]
        public void SidekickIsWarriorLevel5()
        {
            Sidekick = CreateSidekick(SidekickClass.Warrior, 5);
        }

        [Given]
        public void BaseRawDataIsSet()
        {
            BaseRawData = CreateWolfRawData();
        }

        [Then]
        public void ShouldAddStrengthAndConstitutionSavingThrows()
        {
            var saves = Result["save"].AsBsonDocument;
            Assert.That(saves["str"].AsString, Is.EqualTo("+4"));
            Assert.That(saves["con"].AsString, Is.EqualTo("+4"));
        }
    }
}
