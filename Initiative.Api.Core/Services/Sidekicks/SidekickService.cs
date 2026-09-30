using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Models.Parties;
using Initiative.Persistence.Repositories;
using MongoDB.Bson;

namespace Initiative.Api.Core.Services.Sidekicks
{
    public class SidekickService : ISidekickService
    {
        private static readonly IReadOnlySet<string> ValidBaseChallengeRatings =
            new HashSet<string> { "0", "1/8", "1/4", "1/2" };

        private readonly IPartyRepository partyRepository;
        private readonly IBestiaryRepository bestiaryRepository;
        private readonly ISidekickStatBlockBuilder statBlockBuilder;

        public SidekickService(
            IPartyRepository partyRepository,
            IBestiaryRepository bestiaryRepository,
            ISidekickStatBlockBuilder statBlockBuilder)
        {
            this.partyRepository = partyRepository;
            this.bestiaryRepository = bestiaryRepository;
            this.statBlockBuilder = statBlockBuilder;
        }

        public async Task<PartyMember> CreateSidekick(string partyId, string ownerId, string name, SidekickData data, CancellationToken cancellationToken)
        {
            var party = await partyRepository.FetchPartyById(partyId, ownerId, cancellationToken)
                ?? throw new ArgumentException("Party not found.", nameof(partyId));

            var sidekick = await BuildSidekick(name, data, cancellationToken);

            var member = new PartyMember
            {
                Name = name,
                Level = data.Level,
                Sidekick = sidekick
            };

            var members = party.Members.Concat(new[] { member }).ToList();
            await partyRepository.UpdatePartyMembers(partyId, ownerId, members, cancellationToken);

            return member;
        }

        public async Task<PartyMember> UpdateSidekick(string partyId, string ownerId, string memberId, string name, SidekickData data, CancellationToken cancellationToken)
        {
            var party = await partyRepository.FetchPartyById(partyId, ownerId, cancellationToken)
                ?? throw new ArgumentException("Party not found.", nameof(partyId));

            var member = party.Members.FirstOrDefault(m => m.Id == memberId)
                ?? throw new ArgumentException("Party member not found.", nameof(memberId));

            if (member.Sidekick == null)
            {
                throw new ArgumentException("Member is not a sidekick.", nameof(memberId));
            }

            var sidekick = await BuildSidekick(name, data, cancellationToken);
            member.Name = name;
            member.Level = data.Level;
            member.Sidekick = sidekick;

            await partyRepository.UpdatePartyMembers(partyId, ownerId, party.Members, cancellationToken);

            return member;
        }

        public async Task<bool> RemoveSidekick(string partyId, string ownerId, string memberId, CancellationToken cancellationToken)
        {
            var party = await partyRepository.FetchPartyById(partyId, ownerId, cancellationToken);
            if (party == null)
            {
                return false;
            }

            var members = party.Members.Where(m => m.Id != memberId || m.Sidekick == null).ToList();
            if (members.Count == party.Members.Count())
            {
                return false;
            }

            await partyRepository.UpdatePartyMembers(partyId, ownerId, members, cancellationToken);
            return true;
        }

        private async Task<Sidekick> BuildSidekick(string name, SidekickData data, CancellationToken cancellationToken)
        {
            if (data.Level < 1 || data.Level > 20)
            {
                throw new ArgumentOutOfRangeException(nameof(data.Level), "Sidekick level must be between 1 and 20.");
            }

            var baseCreature = await bestiaryRepository.GetCreatureById(data.BaseCreatureId, cancellationToken)
                ?? throw new ArgumentException("Base creature not found.", nameof(data.BaseCreatureId));

            if (!IsValidBaseCreature(baseCreature))
            {
                throw new ArgumentException("Sidekick base creature must have a challenge rating of 1/2 or lower and cannot be legendary.", nameof(data.BaseCreatureId));
            }

            var hitDieSize = SidekickRules.HitDieSize(data.Class);
            var maxHitPoints = ComputeMaxHitPoints(data);

            var sidekick = new Sidekick
            {
                BaseCreatureId = data.BaseCreatureId,
                BaseCreatureName = data.BaseCreatureName ?? baseCreature.Name,
                Class = data.Class,
                Level = data.Level,
                AbilityScores = data.AbilityScores,
                MaxHitPoints = maxHitPoints,
                HitDiceCount = data.Level,
                HitDieSize = hitDieSize,
                HpRolls = data.HpRolls,
                Choices = data.Choices,
                RawData = new BsonDocument()
            };

            if (data.Class == SidekickClass.Spellcaster)
            {
                var kind = data.SpellcasterKind ?? SidekickSpellcasterKind.Magician;
                sidekick.Spellcasting = new SidekickSpellcasting
                {
                    Kind = kind,
                    Ability = SidekickRules.SpellcastingAbility(kind),
                    Cantrips = data.Cantrips,
                    Slots = SidekickRules.SpellSlots(data.Level)
                        .ToDictionary(kv => kv.Key, kv => kv.Value),
                    KnownSpells = data.KnownSpells
                };
            }

            sidekick.RawData = statBlockBuilder.Build(name, sidekick, baseCreature.RawData);

            return sidekick;
        }

        private static bool IsValidBaseCreature(BestiaryCreatureDocument creature)
        {
            if (creature.IsLegendary)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(creature.ChallengeRating))
            {
                return false;
            }

            return ValidBaseChallengeRatings.Contains(creature.ChallengeRating);
        }

        private static int ComputeMaxHitPoints(SidekickData data)
        {
            if (data.HpRolls.Count > 0)
            {
                var total = data.HpRolls.Sum(r => r.Result);
                if (total > 0)
                {
                    return total;
                }
            }

            // Fallback: level 1 max die + average for subsequent levels
            var conModifier = data.AbilityScores.ConstitutionModifier;
            var hitPoints = SidekickRules.HitDieMax(data.Class) + conModifier;
            for (var level = 2; level <= data.Level; level++)
            {
                hitPoints += SidekickRules.HitDieAverage(data.Class) + conModifier;
            }

            return hitPoints;
        }
    }
}
