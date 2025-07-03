using Initiative.Lobby.Core.Dtos;
using LeapingGorilla.Testing.Core.Attributes;


namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public abstract class WhenTestingGetLobbyState : WhenTestingLobbyService
    {
        protected string RoomCode;
        protected EncounterDto Result;

        [When]
        public void GetLobbyStateIsCalled()
        {
            Result = LobbyService.GetLobbyState(RoomCode);
        }
    }
}
