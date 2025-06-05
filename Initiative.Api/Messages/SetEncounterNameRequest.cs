using Newtonsoft.Json;

namespace Initiative.Api.Messages
{
    [JsonObject]
    public class SetEncounterNameRequest
    {
        public required string NewName { get; set; }
    }
}
