﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests
{
    public abstract class WhenTestingLobbyService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected LobbyService LobbyService;

        [Dependency]
        protected IServiceScopeFactory ServiceScopeFactory = Substitute.For<IServiceScopeFactory>();

        protected IInitiativeUserRepository InitiativeUserRepository;
        protected IServiceScope ServiceScope;
        protected IServiceProvider ServiceProvider;
        protected CancellationToken CancellationToken = default;

        protected override void CreateManualDependencies()
        {

            // Setup service provider mocks
            InitiativeUserRepository = Substitute.For<IInitiativeUserRepository>();
            ServiceProvider = Substitute.For<IServiceProvider>();
            ServiceScope = Substitute.For<IServiceScope>();

            ServiceProvider.GetService(typeof(IInitiativeUserRepository))
                .Returns(InitiativeUserRepository);
            
            ServiceScope
                .ServiceProvider
                .Returns(ServiceProvider);

            ServiceScopeFactory
                .CreateScope()
                .Returns(ServiceScope);

            base.CreateManualDependencies();
        }
    }
}
