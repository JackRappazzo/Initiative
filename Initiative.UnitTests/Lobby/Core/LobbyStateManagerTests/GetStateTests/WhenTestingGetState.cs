using Initiative.Lobby.Core.Dtos;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.GetStateTests
{
    public abstract class WhenTestingGetState : WhenTestingLobbyStateManager
    {
        protected EncounterDto Result;

        [When]
        public void GetStateIsCalled()
        {
            Result = LobbyStateManager.GetState(RoomCode);
        }
    }
}