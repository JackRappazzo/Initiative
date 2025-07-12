using System.Linq;
using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.GetStateTests
{
    public class GivenMultipleCallsToSameRoomCode : WhenTestingGetState
    {
        protected EncounterDto SecondResult;
        protected EncounterDto ThirdResult;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .When(GetStateIsCalledMultipleTimes)
            .Then(ShouldReturnConsistentResults);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "MULTI123";
        }

        [When]
        public void GetStateIsCalledMultipleTimes()
        {
            Result = LobbyStateManager.GetState(RoomCode);
            SecondResult = LobbyStateManager.GetState(RoomCode);
            ThirdResult = LobbyStateManager.GetState(RoomCode);
        }

        [Then]
        public void ShouldReturnConsistentResults()
        {
            // All results should be equivalent (same default state)
            Assert.That(Result, Is.Not.Null);
            Assert.That(SecondResult, Is.Not.Null);
            Assert.That(ThirdResult, Is.Not.Null);

            Assert.That(SecondResult.CurrentMode, Is.EqualTo(Result.CurrentMode));
            Assert.That(SecondResult.CurrentCreatureIndex, Is.EqualTo(Result.CurrentCreatureIndex));
            Assert.That(SecondResult.CurrentTurn, Is.EqualTo(Result.CurrentTurn));
            Assert.That(SecondResult.Creatures, Is.EquivalentTo(Result.Creatures));

            Assert.That(ThirdResult.CurrentMode, Is.EqualTo(Result.CurrentMode));
            Assert.That(ThirdResult.CurrentCreatureIndex, Is.EqualTo(Result.CurrentCreatureIndex));
            Assert.That(ThirdResult.CurrentTurn, Is.EqualTo(Result.CurrentTurn));
            Assert.That(ThirdResult.Creatures, Is.EquivalentTo(Result.Creatures));
        }
    }
}