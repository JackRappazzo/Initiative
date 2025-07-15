namespace Initiative.Import.Core.Models
{
    /// <summary>
    /// Represents the various damage types and special resistance cases in D&D
    /// </summary>
    public enum DamageType
    {
        // Standard damage types
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder,
        
        // Special case for dynamic or custom resistances
        Special
    }
}