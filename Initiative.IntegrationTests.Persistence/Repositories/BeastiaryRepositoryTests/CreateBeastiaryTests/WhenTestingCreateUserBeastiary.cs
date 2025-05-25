using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.CreateSystemBeastiaryTests
{
    public abstract class WhenTestingCreateUserBeastiary : WhenTestingBeastiaryRepository
    {
        protected string Result;
        protected string Name;

        [When]
        public async Task CreateIsCalled()
        {
            Result = await BeastiaryRepository.CreateSystemBeastiary(Name, CancellationToken);
        }
    }
}
