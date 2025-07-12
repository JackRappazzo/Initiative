using Initiative.Lobby.Core.Dtos;
using LeapingGorilla.Testing.Core.Attributes;
using System.Threading.Tasks;


namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public abstract class WhenTestingGetLobbyState : WhenTestingLobbyService
    {
        protected string RoomCode;
        protected EncounterDto Result;

        [When]
        public async Task GetLobbyStateIsCalled()
        {
            Result = await LobbyService.GetLobbyState(RoomCode);
        }
    }
}
