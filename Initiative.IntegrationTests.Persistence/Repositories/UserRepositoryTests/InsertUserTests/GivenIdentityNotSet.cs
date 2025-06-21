using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.UserRepositoryTests.InsertUserTests
{
    public class GivenIdentityNotSet : WhenTestingInsertUser
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIsSetWithoutIdentityId)
            .When(InsertUserIsCalled)
            .Then(ShouldThrowArgumentNullException);

        [Given]
        public void UserIsSetWithoutIdentityId()
        {

            this.UserModel = new InitiativeUserModel()
            {
                CreatedAt = DateTime.UtcNow,
                EmailAddress = "email@address.com",
                LastLoggedInAt = null,
                RoomCode = "ROOM123"
            };
        }

        [Then]
        public void ShouldThrowArgumentNullException()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await UserRepository.InsertUser(UserModel, CancellationToken));
            Assert.That(exception.ParamName, Is.EqualTo("IdentityId"));
        }
    }
}
