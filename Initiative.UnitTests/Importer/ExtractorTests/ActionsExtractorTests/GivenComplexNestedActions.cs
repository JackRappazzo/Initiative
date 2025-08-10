using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.ExtractorTests.ActionsExtractorTests
{
    public class GivenComplexNestedActions : WhenTestingExtractActions
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ActionsContainComplexNestedData)
            .When(ExtractActionsIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectActions)
            .And(ShouldHandleNestedListStructures);

        [Given]
        public void ActionsContainComplexNestedData()
        {
            // Test data based on the Fey Step action that was causing the parsing issue
            var complexEntryJson = """
                {
                  "type": "list",
                  "style": "list-hang-notitle",
                  "items": [
                    {
                      "name": "Fuming",
                      "type": "item",
                      "entries": [
                        "The spirit has Advantage on the next attack roll it makes before the end of this turn."
                      ]
                    },
                    {
                      "name": "Mirthful",
                      "type": "item",
                      "entries": [
                        "The target is Charmed by you and the spirit for 1 minute."
                      ]
                    }
                  ]
                }
                """;

            Actions = new List<ActionJson>
            {
                new ActionJson
                {
                    Name = "Fey Step",
                    Entries = new List<JsonElement>
                    {
                        JsonDocument.Parse("\"The spirit magically teleports up to 30 feet to an unoccupied space it can see. Then one of the following effects occurs, based on the spirit's chosen mood:\"").RootElement,
                        JsonDocument.Parse(complexEntryJson).RootElement
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
            Assert.That(Result.Count(), Is.EqualTo(1));
        }

        [Then]
        public void ShouldHandleNestedListStructures()
        {
            var actionsList = Result.ToList();
            
            Assert.That(actionsList[0].Name, Is.EqualTo("Fey Step"));
            Assert.That(actionsList[0].Descriptions.Count(), Is.EqualTo(2));
            
            // First entry should be the simple string
            Assert.That(actionsList[0].Descriptions.First(), Is.EqualTo("The spirit magically teleports up to 30 feet to an unoccupied space it can see. Then one of the following effects occurs, based on the spirit's chosen mood:"));
            
            // Second entry should be the parsed complex structure
            var secondDescription = actionsList[0].Descriptions.Last();
            Assert.That(secondDescription, Contains.Substring("Fuming"));
            Assert.That(secondDescription, Contains.Substring("Advantage"));
            Assert.That(secondDescription, Contains.Substring("Mirthful"));
            Assert.That(secondDescription, Contains.Substring("Charmed"));
        }
    }
}