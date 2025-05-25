using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.CreateUserBeastiaryTests
{
    public abstract class WhenTestingCreateUserBeastiary : WhenTestingBeastiaryRepository
    {
        protected string Result;
        protected string Name;
        protected string OwnerId;

        [When]
        public async Task CreateIsCalled()
        {
            Result = await BeastiaryRepository.CreateUserBeastiary(OwnerId, Name, CancellationToken);
        }
    }
}
