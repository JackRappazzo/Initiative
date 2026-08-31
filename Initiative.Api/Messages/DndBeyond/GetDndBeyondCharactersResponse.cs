namespace Initiative.Api.Messages.DndBeyond
{
    public class GetDndBeyondCharactersResponse
    {
        public IEnumerable<DndBeyondCharacterItem> Characters { get; set; } = [];

        public class DndBeyondCharacterItem
        {
            public long Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
