namespace Initiative.Persistence.Models.Lobby
{
    public class LobbyCreatureStateDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string[] Statuses { get; set; } = [];
        public string HealthStatus { get; set; } = string.Empty;
        public bool IsPlayer { get; set; }
        public bool IsHidden { get; set; }
    }
}