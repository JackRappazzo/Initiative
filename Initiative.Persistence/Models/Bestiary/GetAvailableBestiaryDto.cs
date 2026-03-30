namespace Initiative.Persistence.Models.Bestiary
{
    public class GetAvailableBestiaryDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        /// <summary>
        /// Short source code (e.g. "XMM") for system bestiaries.
        /// Null for user-created custom bestiaries.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Null for system bestiaries. Set to the owner's user ID for user-created bestiaries.
        /// </summary>
        public string? OwnerId { get; set; }
    }
}
