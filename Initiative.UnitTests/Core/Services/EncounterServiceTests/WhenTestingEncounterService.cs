using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Services.Encounters;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests
{
    public abstract class WhenTestingEncounterService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected EncounterService EncounterService;

        [Dependency]
        protected IEncounterRepository EncounterRepository;

        protected CancellationToken CancellationToken = default;
    }
}
