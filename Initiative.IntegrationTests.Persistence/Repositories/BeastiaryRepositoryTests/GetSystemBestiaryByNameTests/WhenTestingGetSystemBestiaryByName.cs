using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.GetSystemBestiaryByNameTests
{
    public abstract class WhenTestingGetSystemBestiaryByName : WhenTestingBeastiaryRepository
    {
        protected string Name;
        protected Bestiary Result;

        [When]
        public async Task GetSystemBestiaryByNameIsCalled()
        {
            Result = await BeastiaryRepository.GetSystemBestiaryByName(Name, CancellationToken);
        }
    }
}