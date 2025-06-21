using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.UserRepositoryTests.InsertUserTests
{
    public class GivenUserExists : WhenTestingInsertUser
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIsSet)
            .Given(UserAlreadyExists)
            .When(InsertUserIsCalled)
            .Then(ShouldThrowInvalidOperationException);

        [Given]
        public void UserIsSet()
        {
            this.UserModel = new InitiativeUserModel()
            {
                CreatedAt = DateTime.UtcNow,
                IdentityId = ObjectId.GenerateNewId().ToString(),
                EmailAddress = "email@address.com",
                LastLoggedInAt = null,
                RoomCode = "ROOM123"
            };
        }

        [Given]
        public async Task UserAlreadyExists()
        {
            await UserRepository.InsertUser(UserModel, CancellationToken);
        }

        [Then]
        public async Task ShouldThrowInvalidOperationException()
        {
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await UserRepository.InsertUser(UserModel, CancellationToken));

            Assert.That(exception.Message, Is.EqualTo($"User with IdentityId {UserModel.IdentityId} already exists."));

        }
    }
}
