using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Services.Bestiaries;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Core.Services.BestiaryServiceTests
{
    public abstract class WhenTestingBestiaryService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected BestiaryService BestiaryService;

        [Dependency]
        protected IBestiaryRepository BestiaryRepository;

        protected CancellationToken CancellationToken = default;
    }
}