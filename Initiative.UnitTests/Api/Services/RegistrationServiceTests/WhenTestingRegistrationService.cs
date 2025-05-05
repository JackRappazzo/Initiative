using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Services;
using Initiative.UnitTests.Api.Helpers;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.RegistrationServiceTests
{
    public abstract class WhenTestingRegistrationService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected UserRegistrationService UserRegistrationService;

        [Dependency]
        protected UserManager<InitiativeUser>  UserManager = Substitute.For<MockUserManager<InitiativeUser>>();

        protected CancellationToken CancellationToken = default;


    }
}
