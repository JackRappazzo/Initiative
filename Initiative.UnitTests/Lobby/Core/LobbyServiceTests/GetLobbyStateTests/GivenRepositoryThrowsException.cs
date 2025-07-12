using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public class GivenRepositoryThrowsException : WhenTestingGetLobbyState
    {
        protected Exception ThrownException;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(RepositoryThrowsException)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnDefaultState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "ERROR123";
        }

        [Given]
        public void RepositoryThrowsException()
        {
            ThrownException = new Exception("Database connection failed");
            LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken)
                .Throws(ThrownException);
        }

        [Then]
        public void ShouldReturnDefaultState()
        {
            // The service should gracefully handle repository errors and return default state
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Empty);
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(0));
            Assert.That(Result.CurrentTurn, Is.EqualTo(0));
            Assert.That(Result.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }
    }
}