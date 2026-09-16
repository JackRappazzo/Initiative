using System.Text.Json;
using Initiative.Api.Core.Services.DndBeyond;

namespace Initiative.UnitTests.Api.Core.Services.DndBeyond
{
    public class DndBeyondProxyServiceTests
    {
        private static JsonElement ParseData(string json)
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.Clone();
        }

        [Test]
        public void CalculateMaxHitPoints_WhenNoOverride_AddsConstitutionModifierPerLevel()
        {
            var data = ParseData("""
                {
                  "baseHitPoints": 22,
                  "overrideHitPoints": null,
                  "stats": [ { "id": 3, "value": 16 } ],
                  "bonusStats": [ { "id": 3, "value": 0 } ],
                  "overrideStats": [ { "id": 3, "value": 0 } ]
                }
                """);

            var result = DndBeyondProxyService.CalculateMaxHitPoints(data, 3);

            Assert.That(result, Is.EqualTo(31));
        }

        [Test]
        public void CalculateMaxHitPoints_WhenOverrideIsSet_UsesOverride()
        {
            var data = ParseData("""
                {
                  "baseHitPoints": 22,
                  "overrideHitPoints": 45,
                  "stats": [ { "id": 3, "value": 16 } ],
                  "bonusStats": [ { "id": 3, "value": 0 } ],
                  "overrideStats": [ { "id": 3, "value": 0 } ]
                }
                """);

            var result = DndBeyondProxyService.CalculateMaxHitPoints(data, 3);

            Assert.That(result, Is.EqualTo(45));
        }

        [Test]
        public void CalculateMaxHitPoints_WithNegativeConstitutionModifier_FloorsCorrectly()
        {
            var data = ParseData("""
                {
                  "baseHitPoints": 8,
                  "overrideHitPoints": null,
                  "stats": [ { "id": 3, "value": 9 } ],
                  "bonusStats": [ { "id": 3, "value": 0 } ],
                  "overrideStats": [ { "id": 3, "value": 0 } ]
                }
                """);

            var result = DndBeyondProxyService.CalculateMaxHitPoints(data, 2);

            Assert.That(result, Is.EqualTo(6));
        }

        [Test]
        public void CalculateMaxHitPoints_WithBonusStats_AddsBonusToConstitutionScore()
        {
            var data = ParseData("""
                {
                  "baseHitPoints": 10,
                  "overrideHitPoints": null,
                  "stats": [ { "id": 3, "value": 14 } ],
                  "bonusStats": [ { "id": 3, "value": 1 } ],
                  "overrideStats": [ { "id": 3, "value": 0 } ]
                }
                """);

            var result = DndBeyondProxyService.CalculateMaxHitPoints(data, 1);

            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void CalculateMaxHitPoints_WithOverrideStats_UsesOverrideScore()
        {
            var data = ParseData("""
                {
                  "baseHitPoints": 10,
                  "overrideHitPoints": null,
                  "stats": [ { "id": 3, "value": 10 } ],
                  "bonusStats": [ { "id": 3, "value": 0 } ],
                  "overrideStats": [ { "id": 3, "value": 18 } ]
                }
                """);

            var result = DndBeyondProxyService.CalculateMaxHitPoints(data, 1);

            Assert.That(result, Is.EqualTo(14));
        }
    }
}
