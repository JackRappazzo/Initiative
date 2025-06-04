using Newtonsoft.Json;

namespace Initiative.Api.Messages
{
    [JsonObject]
    public class CreatureJsonModel
    {
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int ArmorClass { get; set; }
    }
}
