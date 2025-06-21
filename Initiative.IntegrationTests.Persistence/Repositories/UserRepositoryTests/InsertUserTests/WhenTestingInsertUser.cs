using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.UserRepositoryTests.InsertUserTests
{
    public abstract class WhenTestingInsertUser : WhenTestingUserRepository
    {
        protected InitiativeUserModel UserModel;
        protected string Result;

        [When(DoNotRethrowExceptions:true)]
        public async Task InsertUserIsCalled()
        {
            Result = await UserRepository.InsertUser(UserModel, CancellationToken);
        }
    }
}
