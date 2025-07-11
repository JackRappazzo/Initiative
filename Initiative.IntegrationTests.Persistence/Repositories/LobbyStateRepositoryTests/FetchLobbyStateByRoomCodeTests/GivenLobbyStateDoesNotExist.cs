using System;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.FetchLobbyStateByRoomCodeTests
{
    public class GivenLobbyStateDoesNotExist : WhenTestingFetchLobbyStateByRoomCode
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NonExistentRoomCodeIsSet)
            .When(FetchLobbyStateByRoomCodeIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void NonExistentRoomCodeIsSet()
        {
            RoomCode = "NONEXISTENT";
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}