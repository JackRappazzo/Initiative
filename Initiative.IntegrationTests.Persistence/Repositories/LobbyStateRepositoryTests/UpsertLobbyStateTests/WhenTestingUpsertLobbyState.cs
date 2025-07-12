using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.UpsertLobbyStateTests
{
    public abstract class WhenTestingUpsertLobbyState : WhenTestingLobbyStateRepository
    {
        protected string RoomCode;
        protected string[] Creatures;
        protected int TurnNumber;
        protected int CurrentCreatureIndex;
        protected LobbyMode CurrentMode;
        protected string Result;

        [When]
        public async Task UpsertLobbyStateIsCalled()
        {
            Result = await LobbyStateRepository.UpsertLobbyState(RoomCode, Creatures, TurnNumber, CurrentCreatureIndex, CurrentMode, CancellationToken);
        }
    }
}