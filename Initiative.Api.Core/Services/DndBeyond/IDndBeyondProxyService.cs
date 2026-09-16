using System.Net;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondCharacterListItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }

    public class DndBeyondSpellSlot
    {
        public int Level { get; set; }
        public int Used { get; set; }
        public int Available { get; set; }
    }

    public class DndBeyondPreparedSpell
    {
        public string Name { get; set; } = "";
        public int Level { get; set; }
        public bool IsCantrip { get; set; }
    }

    public class DndBeyondResource
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }

    public class DndBeyondAttack
    {
        public string Name { get; set; } = "";
        public int ToHitBonus { get; set; }
        public string DamageDice { get; set; } = "";
        public int DamageBonus { get; set; }
        public string? DamageType { get; set; }
        public int Range { get; set; }
        public int LongRange { get; set; }
        public bool IsRanged { get; set; }
    }

    public class DndBeyondCharacterDetail
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public string? ClassName { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int TemporaryHP { get; set; }

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int ArmorClass { get; set; }
        public int ProficiencyBonus { get; set; }
        public string? Race { get; set; }
        public int Speed { get; set; }

        public List<DndBeyondSpellSlot> SpellSlots { get; set; } = new();
        public List<DndBeyondSpellSlot> PactSlots { get; set; } = new();
        public List<DndBeyondPreparedSpell> PreparedSpells { get; set; } = new();
        public DndBeyondResource? FocusPoints { get; set; }
        public List<DndBeyondAttack> Attacks { get; set; } = new();
    }

    public class DndBeyondRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public TimeSpan? RetryAfter { get; }

        public DndBeyondRequestException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public DndBeyondRequestException(HttpStatusCode statusCode, string message, TimeSpan? retryAfter) : base(message)
        {
            StatusCode = statusCode;
            RetryAfter = retryAfter;
        }
    }

    public interface IDndBeyondProxyService
    {
        Task<IEnumerable<DndBeyondCharacterListItem>> GetCharacters(string token, CancellationToken cancellationToken);

        Task<DndBeyondCharacterDetail?> GetCharacter(string token, string characterId, CancellationToken cancellationToken);
    }
}
