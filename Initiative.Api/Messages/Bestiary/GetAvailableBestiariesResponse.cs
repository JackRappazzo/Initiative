namespace Initiative.Api.Messages.Bestiary
{
    public class GetAvailableBestiariesResponse
    {
        public required IEnumerable<BestiaryItem> Bestiaries { get; set; }

        public class BestiaryItem
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public string? Source { get; set; }
            public string? OwnerId { get; set; }
        }
    }
}
