namespace Initiative.Api.Messages.Bestiary
{
    public class SaveCustomCreatureRequest
    {
        public required string Name { get; set; }
        public string? CreatureType { get; set; }
        public string? ChallengeRating { get; set; }
        public bool IsLegendary { get; set; }
        public int? HP { get; set; }
        public int? AC { get; set; }
        public string? Traits { get; set; }
    }
}
