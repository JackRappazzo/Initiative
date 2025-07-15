using Initiative.Import.Core.Models;
using Initiative.Import.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenComplexConditionImmunitiesExtraction : WhenTestingCreatureMapper
    {
        private List<JsonElement> _conditionElements;
        private List<string> _result;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ComplexConditionImmunitiesAreProvided)
            .When(ExtractConditionImmunitiesIsCalledPublicly)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractOnlyConditionNames)
            .And(ShouldIgnoreNotesAndCondFields);

        [Given]
        public void ComplexConditionImmunitiesAreProvided()
        {
            // Create a mix of simple strings and complex objects
            var complexConditionJson = """
            {
                "conditionImmune": [
                    "charmed"
                ],
                "note": "(with Mind Blank)",
                "cond": true
            }
            """;
            
            _conditionElements = new List<JsonElement>
            {
                JsonDocument.Parse("\"exhaustion\"").RootElement, // Simple string
                JsonDocument.Parse("\"poisoned\"").RootElement,   // Simple string
                JsonDocument.Parse(complexConditionJson).RootElement // Complex object
            };
        }

        [When]
        public void ExtractConditionImmunitiesIsCalledPublicly()
        {
            _result = CreatureMapper.ExtractConditionImmunitiesPublic(_conditionElements);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractOnlyConditionNames()
        {
            Assert.That(_result, Is.Not.Null);
            Assert.That(_result, Has.Count.EqualTo(3));
            Assert.That(_result, Contains.Item("exhaustion"));
            Assert.That(_result, Contains.Item("poisoned"));
            Assert.That(_result, Contains.Item("charmed"));
        }

        [Then]
        public void ShouldIgnoreNotesAndCondFields()
        {
            // The result should not contain any of the ignored fields
            Assert.That(_result, Does.Not.Contain("(with Mind Blank)"));
            Assert.That(_result, Does.Not.Contain("true"));
            Assert.That(_result, Does.Not.Contain("note"));
            Assert.That(_result, Does.Not.Contain("cond"));
        }
    }
}