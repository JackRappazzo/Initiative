namespace Initiative.Api.Messages.DndBeyond
{
    public class GetDndBeyondCharacterResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public string? ClassName { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int TemporaryHP { get; set; }
    }
}
