namespace Initiative.Api.Messages
{
    public class GetBestiaryStubResponse
    {
        public required string Name { get; set; }
        public required string Id { get; set; }

        public required bool IsPublic { get; set; }
    }
}
