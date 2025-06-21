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
    public class GivenHappyPath : WhenTestingInsertUser
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIsSet)
            .Given(UserDoesNotAlreadyExist)
            .When(InsertUserIsCalled)
            .Then(ShouldReturnId);

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
        public void UserDoesNotAlreadyExist()
        {
            //Intentionally left blank. There should not be another user with the same identity ID
        }

        [Then]
        public void ShouldReturnId()
        {
            ObjectId objectId;
            ObjectId.TryParse(this.Result, out objectId);

            Assert.That(!string.IsNullOrEmpty(Result), "Result should not be null or empty");
            Assert.That(objectId, Is.Not.EqualTo(ObjectId.Empty), "Result should be a valid ObjectId");
        }
    }
}
