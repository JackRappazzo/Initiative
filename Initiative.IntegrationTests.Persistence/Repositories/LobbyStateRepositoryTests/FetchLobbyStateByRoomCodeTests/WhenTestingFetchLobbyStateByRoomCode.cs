using System.Threading.Tasks;
using Initiative.Persistence.Models.Lobby;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.FetchLobbyStateByRoomCodeTests
{
    public abstract class WhenTestingFetchLobbyStateByRoomCode : WhenTestingLobbyStateRepository
    {
        protected string RoomCode;
        protected LobbyStateDto Result;

        [When]
        public async Task FetchLobbyStateByRoomCodeIsCalled()
        {
            Result = await LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken);
        }
    }
}