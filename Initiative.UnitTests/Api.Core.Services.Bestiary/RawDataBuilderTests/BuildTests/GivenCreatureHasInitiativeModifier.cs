using Initiative.Api.Core.Services.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.RawDataBuilderTests.BuildTests
{
    public class GivenCreatureHasInitiativeModifier : WhenTestingBuild
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CreatureHasInitiativeModifierSet)
            .When(BuildIsCalled)
            .Then(ShouldIncludeInitiativeModifierInRawData);

        [Given]
        public void CreatureHasInitiativeModifierSet()
        {
            Data = new CustomCreatureData
            {
                Name = "Assassin",
                InitiativeModifier = 7
            };
        }

        [Then]
        public void ShouldIncludeInitiativeModifierInRawData()
        {
            Assert.That(Result["initiativeModifier"].AsInt32, Is.EqualTo(7));
        }
    }
}
