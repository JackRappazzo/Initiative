using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.UpsertLobbyStateTests
{
    public abstract class WhenTestingUpsertLobbyState : WhenTestingLobbyStateRepository
    {
        protected string RoomCode;
        protected string[] Creatures;
        protected int TurnNumber;
        protected int CurrentCreatureIndex;
        protected string Result;

        [When]
        public async Task UpsertLobbyStateIsCalled()
        {
            Result = await LobbyStateRepository.UpsertLobbyState(RoomCode, Creatures, TurnNumber, CurrentCreatureIndex, CancellationToken);
        }
    }
}