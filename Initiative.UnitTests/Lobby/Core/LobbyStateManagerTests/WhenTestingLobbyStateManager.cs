using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests
{
    public abstract class WhenTestingLobbyStateManager : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected LobbyStateManager LobbyStateManager;

        protected string RoomCode;
    }
}