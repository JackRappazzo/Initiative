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

        [Test]
        public void CalculateArmorClass_WithLightArmor_AddsDexterity()
        {
            var data = ParseData("""
                {
                  "stats": [ { "id": 2, "value": 15 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    { "equipped": true, "definition": { "armorClass": 12, "type": "Light Armor" } }
                  ],
                  "modifiers": { "item": [], "class": [], "race": [], "background": [], "feat": [], "condition": [] }
                }
                """);

            var result = DndBeyondProxyService.CalculateArmorClass(data);

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void CalculateArmorClass_WithUnarmoredDefense_UsesSecondaryStat()
        {
            var data = ParseData("""
                {
                  "stats": [ { "id": 2, "value": 15 }, { "id": 5, "value": 13 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "modifiers": {
                    "class": [ { "type": "set", "subType": "unarmored-armor-class", "statId": 5, "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.CalculateArmorClass(data);

            Assert.That(result, Is.EqualTo(13));
        }

        [Test]
        public void CalculateArmorClass_WithArmorClassBonus_AddsBonus()
        {
            var data = ParseData("""
                {
                  "stats": [ { "id": 2, "value": 15 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    { "equipped": true, "definition": { "armorClass": 12, "type": "Light Armor" } }
                  ],
                  "modifiers": {
                    "item": [ { "type": "bonus", "subType": "armor-class", "value": 1, "isGranted": true } ],
                    "class": [], "race": [], "background": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.CalculateArmorClass(data);

            Assert.That(result, Is.EqualTo(15));
        }

        [Test]
        public void CalculateSpeed_AddsMovementModifiers()
        {
            var data = ParseData("""
                {
                  "race": { "weightSpeeds": { "normal": { "walk": 30 } } },
                  "modifiers": {
                    "class": [ { "type": "bonus", "subType": "unarmored-movement", "value": 10, "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.CalculateSpeed(data);

            Assert.That(result, Is.EqualTo(40));
        }

        [Test]
        public void ParseSpellSlots_ExcludesEmptyLevels()
        {
            var data = ParseData("""
                {
                  "spellSlots": [
                    { "level": 1, "used": 3, "available": 0 },
                    { "level": 2, "used": 0, "available": 0 },
                    { "level": 3, "used": 1, "available": 2 }
                  ]
                }
                """);

            var result = DndBeyondProxyService.ParseSpellSlots(data, "spellSlots");

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Level, Is.EqualTo(1));
            Assert.That(result[0].Used, Is.EqualTo(3));
            Assert.That(result[1].Level, Is.EqualTo(3));
            Assert.That(result[1].Available, Is.EqualTo(2));
        }

        [Test]
        public void ParsePreparedSpells_FiltersAndDeduplicates()
        {
            var data = ParseData("""
                {
                  "classSpells": [
                    {
                      "spells": [
                        { "definition": { "name": "Eldritch Blast", "level": 0 }, "countsAsKnownSpell": true, "prepared": false, "alwaysPrepared": false },
                        { "definition": { "name": "Arms of Hadar", "level": 1 }, "countsAsKnownSpell": true, "prepared": false, "alwaysPrepared": false },
                        { "definition": { "name": "Not Known", "level": 1 }, "countsAsKnownSpell": false, "prepared": false, "alwaysPrepared": false }
                      ]
                    }
                  ],
                  "spells": {
                    "class": [
                      { "definition": { "name": "Calm Emotions", "level": 2 }, "countsAsKnownSpell": false, "prepared": false, "alwaysPrepared": true },
                      { "definition": { "name": "Eldritch Blast", "level": 0 }, "countsAsKnownSpell": false, "prepared": false, "alwaysPrepared": true }
                    ],
                    "race": [], "feat": [], "background": [], "item": []
                  }
                }
                """);

            var result = DndBeyondProxyService.ParsePreparedSpells(data);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Name, Is.EqualTo("Eldritch Blast"));
            Assert.That(result[0].IsCantrip, Is.True);
            Assert.That(result[1].Name, Is.EqualTo("Arms of Hadar"));
            Assert.That(result[2].Name, Is.EqualTo("Calm Emotions"));
        }

        [Test]
        public void ParseFocusPoints_ReadsLimitedUse()
        {
            var data = ParseData("""
                {
                  "actions": {
                    "class": [
                      { "name": "Focus Points", "limitedUse": { "maxUses": 5, "numberUsed": 2 } }
                    ]
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseFocusPoints(data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Max, Is.EqualTo(5));
            Assert.That(result.Current, Is.EqualTo(3));
        }

        [Test]
        public void ParseFocusPoints_WhenAbsent_ReturnsNull()
        {
            var data = ParseData("""
                {
                  "actions": {
                    "class": [
                      { "name": "Unarmed Strike", "limitedUse": null }
                    ]
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseFocusPoints(data);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ParseAttacks_MeleeWeapon_UsesStrengthAndProficiency()
        {
            var data = ParseData("""
                {
                  "classes": [ { "level": 5 } ],
                  "stats": [ { "id": 1, "value": 16 }, { "id": 2, "value": 10 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    {
                      "equipped": true,
                      "definition": {
                        "name": "Longsword",
                        "baseItemId": 4,
                        "categoryId": 2,
                        "attackType": 1,
                        "range": 5,
                        "damage": { "diceString": "1d8" },
                        "damageType": "Slashing",
                        "properties": [],
                        "grantedModifiers": []
                      }
                    }
                  ],
                  "modifiers": {
                    "class": [ { "type": "proficiency", "subType": "martial-weapons", "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseAttacks(data);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Longsword"));
            Assert.That(result[0].IsRanged, Is.False);
            Assert.That(result[0].ToHitBonus, Is.EqualTo(6));
            Assert.That(result[0].DamageBonus, Is.EqualTo(3));
            Assert.That(result[0].DamageDice, Is.EqualTo("1d8"));
        }

        [Test]
        public void ParseAttacks_RangedWeapon_UsesDexterity()
        {
            var data = ParseData("""
                {
                  "classes": [ { "level": 5 } ],
                  "stats": [ { "id": 1, "value": 8 }, { "id": 2, "value": 16 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    {
                      "equipped": true,
                      "definition": {
                        "name": "Longbow",
                        "baseItemId": 37,
                        "categoryId": 2,
                        "attackType": 2,
                        "range": 150,
                        "longRange": 600,
                        "damage": { "diceString": "1d8" },
                        "damageType": "Piercing",
                        "properties": [],
                        "grantedModifiers": []
                      }
                    }
                  ],
                  "modifiers": {
                    "class": [ { "type": "proficiency", "subType": "martial-weapons", "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseAttacks(data);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].IsRanged, Is.True);
            Assert.That(result[0].ToHitBonus, Is.EqualTo(6));
            Assert.That(result[0].DamageBonus, Is.EqualTo(3));
            Assert.That(result[0].Range, Is.EqualTo(150));
            Assert.That(result[0].LongRange, Is.EqualTo(600));
        }

        [Test]
        public void ParseAttacks_MagicWeapon_AddsMagicBonus()
        {
            var data = ParseData("""
                {
                  "classes": [ { "level": 5 } ],
                  "stats": [ { "id": 1, "value": 16 }, { "id": 2, "value": 10 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    {
                      "equipped": true,
                      "definition": {
                        "name": "Longsword, +1",
                        "baseItemId": 4,
                        "categoryId": 2,
                        "attackType": 1,
                        "range": 5,
                        "damage": { "diceString": "1d8" },
                        "damageType": "Slashing",
                        "properties": [],
                        "grantedModifiers": [ { "type": "bonus", "subType": "magic", "value": 1 } ]
                      }
                    }
                  ],
                  "modifiers": {
                    "class": [ { "type": "proficiency", "subType": "martial-weapons", "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseAttacks(data);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ToHitBonus, Is.EqualTo(7));
            Assert.That(result[0].DamageBonus, Is.EqualTo(4));
        }

        [Test]
        public void ParseAttacks_IgnoresNonWeaponsAndUnequipped()
        {
            var data = ParseData("""
                {
                  "classes": [ { "level": 5 } ],
                  "stats": [ { "id": 1, "value": 16 }, { "id": 2, "value": 10 } ],
                  "bonusStats": [],
                  "overrideStats": [],
                  "inventory": [
                    { "equipped": true, "definition": { "name": "Backpack", "damage": null } },
                    {
                      "equipped": false,
                      "definition": {
                        "name": "Longsword",
                        "baseItemId": 4,
                        "categoryId": 2,
                        "attackType": 1,
                        "range": 5,
                        "damage": { "diceString": "1d8" },
                        "damageType": "Slashing",
                        "properties": [],
                        "grantedModifiers": []
                      }
                    }
                  ],
                  "modifiers": {
                    "class": [ { "type": "proficiency", "subType": "martial-weapons", "isGranted": true } ],
                    "race": [], "background": [], "item": [], "feat": [], "condition": []
                  }
                }
                """);

            var result = DndBeyondProxyService.ParseAttacks(data);

            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
