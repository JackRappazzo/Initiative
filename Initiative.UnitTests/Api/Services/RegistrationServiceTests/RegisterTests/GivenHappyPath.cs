using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Persistence.Extensions;
using Initiative.Persistence.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Api.Services.UserServiceTests.RegisterTests
{
    public class GivenHappyPath : WhenTestingRegister
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameEmailAndPasswordAreSet)
            .And(EmailIsNotInUse)
            .And(UserCanBeCreated)
            .When(RegisterIsCalled)
            .Then(ShouldReturnSuccess)
            .And(ShouldCallGenerateCode)
            .And(ShouldCallInsertUser)
            .And(ShouldNotReturnErrorMessage);

        [Given]
        public void NameEmailAndPasswordAreSet()
        {
            DisplayName = "TestUser";
            Email = "email@address.com";
            Password = "password";
        }

        [Given]
        public void EmailIsNotInUse()
        {
            UserManager.FindByEmailAsync(Email)
                .ReturnsNull();
        }

        [Given]
        public void UserCanBeCreated()
        {
            UserManager.CreateAsync(Arg.Is<ApplicationIdentity>(u => u.Email == Email), Password)
                .Returns(IdentityResult.Success)
                .AndDoes(callInfo =>
                {
                    callInfo
                    .Arg<ApplicationIdentity>()
                    .Id = ObjectId.GenerateNewId(); // Simulate setting the user ID
                });
        }

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(IsSuccess, Is.True);
        }

        [Then]
        public void ShouldCallInsertUser()
        {
            UserRepository.Received(1).InsertUser(Arg.Is<InitiativeUserModel>(u =>
                u.EmailAddress == Email &&
                u.IdentityId.IsValidObjectId()),
                CancellationToken);
        }

        [Then]
        public void ShouldCallGenerateCode()
        {
            Base62CodeGenerator.Received(1).GenerateCode(Arg.Any<int>());
        }

        [Then]
        public void ShouldNotReturnErrorMessage()
        {
            Assert.That(string.IsNullOrEmpty(ErrorMessage), Is.True);
        }
    }
}
