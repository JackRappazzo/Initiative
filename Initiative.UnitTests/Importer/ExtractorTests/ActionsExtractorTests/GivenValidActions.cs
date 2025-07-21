using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ActionsExtractorTests
{
    public class GivenValidActions : WhenTestingExtractActions
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ActionsContainValidData)
            .When(ExtractActionsIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectActions)
            .And(ShouldMapNamesAndDescriptions);

        [Given]
        public void ActionsContainValidData()
        {
            Actions = new List<ActionJson>
            {
                new ActionJson
                {
                    Name = "Bite",
                    Entries = new List<string>
                    {
                        "Melee Weapon Attack: +5 to hit, reach 5 ft., one target.",
                        "Hit: 8 (1d8 + 4) piercing damage."
                    }
                },
                new ActionJson
                {
                    Name = "Claw",
                    Entries = new List<string>
                    {
                        "Melee Weapon Attack: +5 to hit, reach 5 ft., one target.",
                        "Hit: 6 (1d6 + 3) slashing damage."
                    }
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectActions()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(2));
        }

        [Then]
        public void ShouldMapNamesAndDescriptions()
        {
            var actionsList = Result.ToList();
            
            Assert.That(actionsList[0].Name, Is.EqualTo("Bite"));
            Assert.That(actionsList[0].Descriptions.Count(), Is.EqualTo(2));
            Assert.That(actionsList[0].Descriptions.First(), Is.EqualTo("Melee Weapon Attack: +5 to hit, reach 5 ft., one target."));
            
            Assert.That(actionsList[1].Name, Is.EqualTo("Claw"));
            Assert.That(actionsList[1].Descriptions.Count(), Is.EqualTo(2));
        }
    }
}