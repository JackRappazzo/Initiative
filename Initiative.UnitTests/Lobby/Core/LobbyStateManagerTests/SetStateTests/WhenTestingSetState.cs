using Initiative.Lobby.Core.Dtos;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.SetStateTests
{
    public abstract class WhenTestingSetState : WhenTestingLobbyStateManager
    {
        protected EncounterDto EncounterDto;

        [When]
        public void SetStateIsCalled()
        {
            LobbyStateManager.SetState(RoomCode, EncounterDto);
        }
    }
}