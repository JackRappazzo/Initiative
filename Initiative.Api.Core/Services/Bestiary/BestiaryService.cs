using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;
using MongoDB.Bson;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class BestiaryService : IBestiaryService
    {
        private readonly IBestiaryRepository _repository;

        public BestiaryService(IBestiaryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken)
            => _repository.GetBestariesByOwners(new string?[] { null, userId }, cancellationToken);

        public async Task<SearchCreaturesResult> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
        {
            var countQuery = new BestiarySearchQuery
            {
                BestiaryIds = query.BestiaryIds,
                NameSearch = query.NameSearch,
                CreatureType = query.CreatureType,
                ChallengeRating = query.ChallengeRating,
                IsLegendary = query.IsLegendary,
                OwnerId = query.OwnerId,
            };

            var creaturesTask = _repository.SearchCreatures(query, cancellationToken);
            var countTask = _repository.CountCreatures(countQuery, cancellationToken);
            await Task.WhenAll(creaturesTask, countTask);

            return new SearchCreaturesResult { Creatures = creaturesTask.Result, TotalCount = countTask.Result };
        }

        public Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken)
            => _repository.GetCreatureById(creatureId, cancellationToken);

        public async Task<BestiaryDocument> CreateCustomBestiary(string userId, string name, CancellationToken cancellationToken)
        {
            var doc = new BestiaryDocument { Name = name, OwnerId = userId };
            await _repository.CreateBestiary(doc, cancellationToken);
            return doc;
        }

        public async Task RenameBestiary(string bestiaryId, string userId, string name, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));
            await _repository.RenameBestiary(bestiaryId, userId, name, cancellationToken);
        }

        public async Task DeleteBestiary(string bestiaryId, string userId, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));
            await _repository.DeleteBestiary(bestiaryId, userId, cancellationToken);
        }

        public async Task<BestiaryCreatureDocument> CreateCustomCreature(string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));

            var doc = new BestiaryCreatureDocument
            {
                BestiaryId = bestiaryId,
                Name = data.Name,
                CreatureType = data.CreatureType,
                ChallengeRating = data.ChallengeRating,
                IsLegendary = data.IsLegendary,
                RawData = BuildRawData(data)
            };

            await _repository.InsertCreature(doc, cancellationToken);
            return doc;
        }

        public async Task UpdateCustomCreature(string creatureId, string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));

            var existing = await _repository.GetCreatureById(creatureId, cancellationToken)
                ?? throw new ArgumentException("Creature not found.", nameof(creatureId));

            existing.Name = data.Name;
            existing.CreatureType = data.CreatureType;
            existing.ChallengeRating = data.ChallengeRating;
            existing.IsLegendary = data.IsLegendary;
            existing.RawData = BuildRawData(data);

            await _repository.UpdateCreature(existing, cancellationToken);
        }

        public Task DeleteCustomCreature(string creatureId, CancellationToken cancellationToken)
            => _repository.DeleteCreature(creatureId, cancellationToken);

        private static BsonDocument BuildRawData(CustomCreatureData data)
        {
            var doc = new BsonDocument { { "name", data.Name } };

            // Identity
            if (data.Size != null)
                doc["size"] = new BsonArray { data.Size };

            if (data.CreatureType != null || data.Subtype != null)
            {
                if (data.Subtype != null)
                    doc["type"] = new BsonDocument { { "type", data.CreatureType ?? string.Empty }, { "tags", new BsonArray { data.Subtype } } };
                else
                    doc["type"] = data.CreatureType != null ? (BsonValue)data.CreatureType : BsonNull.Value;
            }

            if (data.Alignment != null)
                doc["alignment"] = new BsonArray { data.Alignment };

            if (data.ChallengeRating != null)
                doc["cr"] = data.ChallengeRating;

            if (data.ProficiencyBonus.HasValue)
                doc["pb"] = data.ProficiencyBonus.Value;

            // HP
            if (data.HP.HasValue || data.HitDice != null)
            {
                var hpDoc = new BsonDocument();
                if (data.HP.HasValue) hpDoc["average"] = data.HP.Value;
                if (data.HitDice != null) hpDoc["formula"] = data.HitDice;
                doc["hp"] = hpDoc;
            }

            // AC
            if (data.AC.HasValue)
            {
                if (data.AcNote != null)
                    doc["ac"] = new BsonArray { new BsonDocument { { "ac", data.AC.Value }, { "from", new BsonArray { data.AcNote } } } };
                else
                    doc["ac"] = new BsonArray { data.AC.Value };
            }

            // Ability scores
            var ab = data.AbilityScores;
            if (ab != null)
            {
                if (ab.Str.HasValue) doc["str"] = ab.Str.Value;
                if (ab.Dex.HasValue) doc["dex"] = ab.Dex.Value;
                if (ab.Con.HasValue) doc["con"] = ab.Con.Value;
                if (ab.Int.HasValue) doc["int"] = ab.Int.Value;
                if (ab.Wis.HasValue) doc["wis"] = ab.Wis.Value;
                if (ab.Cha.HasValue) doc["cha"] = ab.Cha.Value;
            }

            // Speed
            var spd = data.Speed;
            if (spd != null)
            {
                var spdDoc = new BsonDocument();
                if (spd.Walk.HasValue) spdDoc["walk"] = spd.Walk.Value;
                if (spd.Fly.HasValue)
                {
                    if (spd.CanHover == true)
                        spdDoc["fly"] = new BsonDocument { { "number", spd.Fly.Value }, { "condition", "(hover)" } };
                    else
                        spdDoc["fly"] = spd.Fly.Value;
                }
                if (spd.Swim.HasValue) spdDoc["swim"] = spd.Swim.Value;
                if (spd.Burrow.HasValue) spdDoc["burrow"] = spd.Burrow.Value;
                if (spd.Climb.HasValue) spdDoc["climb"] = spd.Climb.Value;
                if (spdDoc.ElementCount > 0) doc["speed"] = spdDoc;
            }

            // Saving throws
            if (data.SavingThrows?.Count > 0)
            {
                var saveDoc = new BsonDocument();
                foreach (var kv in data.SavingThrows) saveDoc[kv.Key] = kv.Value;
                doc["save"] = saveDoc;
            }

            // Skills
            if (data.Skills?.Count > 0)
            {
                var skillDoc = new BsonDocument();
                foreach (var kv in data.Skills) skillDoc[kv.Key] = kv.Value;
                doc["skill"] = skillDoc;
            }

            // Damage / condition modifiers
            if (data.DamageResistances?.Count > 0)
                doc["resist"] = new BsonArray(data.DamageResistances.Select(r => (BsonValue)r));

            if (data.DamageImmunities?.Count > 0)
                doc["immune"] = new BsonArray(data.DamageImmunities.Select(r => (BsonValue)r));

            if (data.DamageVulnerabilities?.Count > 0)
                doc["vulnerable"] = new BsonArray(data.DamageVulnerabilities.Select(r => (BsonValue)r));

            if (data.ConditionImmunities?.Count > 0)
                doc["conditionImmune"] = new BsonArray(data.ConditionImmunities.Select(r => (BsonValue)r));

            // Senses / languages
            if (data.Senses?.Count > 0)
                doc["senses"] = new BsonArray(data.Senses.Select(s => (BsonValue)s));

            if (data.Languages?.Count > 0)
                doc["languages"] = new BsonArray(data.Languages.Select(l => (BsonValue)l));

            // Traits (free text, stored in a single entry)
            if (!string.IsNullOrWhiteSpace(data.Traits))
                doc["trait"] = new BsonArray
                {
                    new BsonDocument { { "name", "Traits" }, { "entries", new BsonArray { data.Traits } } }
                };

            // Action sections
            static BsonArray EntriesToBson(List<CustomCreatureEntry> entries) =>
                new(entries.Select(e => (BsonValue)new BsonDocument
                {
                    { "name", e.Name },
                    { "entries", new BsonArray { e.Description } }
                }));

            if (data.Actions?.Count > 0) doc["action"] = EntriesToBson(data.Actions);
            if (data.BonusActions?.Count > 0) doc["bonus"] = EntriesToBson(data.BonusActions);
            if (data.Reactions?.Count > 0) doc["reaction"] = EntriesToBson(data.Reactions);
            if (data.LegendaryActions?.Count > 0) doc["legendary"] = EntriesToBson(data.LegendaryActions);
            if (data.LegendaryActionCount.HasValue) doc["legendaryActionsLair"] = data.LegendaryActionCount.Value;

            // Spellcasting
            var sc = data.Spellcasting;
            if (sc != null && (sc.SlotSpells?.Count > 0 || sc.DailySpells?.Count > 0))
            {
                var scDoc = new BsonDocument { { "name", "Spellcasting" }, { "type", "spellcasting" } };
                if (sc.Ability != null) scDoc["ability"] = sc.Ability;

                var headerParts = new List<string>();
                if (sc.SpellSaveDc.HasValue) headerParts.Add($"spell save DC {sc.SpellSaveDc.Value}");
                if (sc.SpellAttackBonus.HasValue) headerParts.Add($"+{sc.SpellAttackBonus.Value} to spell attack rolls");
                if (headerParts.Count > 0)
                    scDoc["headerEntries"] = new BsonArray { $"The creature has the following spells ({string.Join(", ", headerParts)}):" };

                // Slot-based: level 0 = at-will/cantrips
                if (sc.SlotSpells?.Count > 0)
                {
                    var spellsDoc = new BsonDocument();
                    foreach (var kv in sc.SlotSpells)
                    {
                        var spellArray = new BsonArray(kv.Value.Select(s => (BsonValue)s));
                        if (kv.Key == "0")
                            scDoc["will"] = spellArray;
                        else
                            spellsDoc[kv.Key] = new BsonDocument { { "slots", int.TryParse(kv.Key, out var lvl) ? lvl : 1 }, { "spells", spellArray } };
                    }
                    if (spellsDoc.ElementCount > 0) scDoc["spells"] = spellsDoc;
                }

                // X/day
                if (sc.DailySpells?.Count > 0)
                {
                    var dailyDoc = new BsonDocument();
                    foreach (var entry in sc.DailySpells)
                    {
                        // Format: "3/day: Fireball" or "1/day each: Misty Step, Fly"
                        var match = System.Text.RegularExpressions.Regex.Match(entry, @"^(\d+)/day(?:\s+each)?:\s*(.+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var count = match.Groups[1].Value;
                            var spells = match.Groups[2].Value.Split(',').Select(s => (BsonValue)s.Trim()).ToList();
                            var key = count + "e"; // 5etools daily key convention: "3e" = 3/day each
                            dailyDoc[key] = new BsonArray(spells);
                        }
                    }
                    if (dailyDoc.ElementCount > 0) scDoc["daily"] = dailyDoc;
                }

                doc["spellcasting"] = new BsonArray { scDoc };
            }

            return doc;
        }
    }
}
