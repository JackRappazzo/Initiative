using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Api.Services.UserServiceTests.RegisterTests
{
    public class GivenInsertingUserFails : WhenTestingRegister
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameEmailAndPasswordAreSet)
            .And(EmailIsNotInUse)
            .And(IdentityCanBeCreated)
            .And(NoExistingInitiativeUser)
            .And(InsertUserThrows)
            .And(IdentityCanBeDeleted)
            .When(RegisterIsCalled)
            .Then(ShouldNotReturnSuccess)
            .And(ShouldReturnExceptionMessage)
            .And(ShouldDeleteIdentity);

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
        public void IdentityCanBeCreated()
        {
            UserManager.CreateAsync(Arg.Is<ApplicationIdentity>(u => u.Email == Email), Password)
                .Returns(IdentityResult.Success)
                .AndDoes(callInfo =>
                {
                    callInfo
                    .Arg<ApplicationIdentity>()
                    .Id = ObjectId.GenerateNewId();
                });
        }

        [Given]
        public void NoExistingInitiativeUser()
        {
            UserRepository.FetchUserByIdentityId(Arg.Any<string>(), CancellationToken)
                .ReturnsNull();
        }

        [Given]
        public void InsertUserThrows()
        {
            UserRepository.InsertUser(Arg.Any<Initiative.Persistence.Models.InitiativeUserModel>(), CancellationToken)
                .ThrowsAsync(new System.Exception("boom"));
        }

        [Given]
        public void IdentityCanBeDeleted()
        {
            UserManager.FindByIdAsync(Arg.Any<string>())
                .Returns(new ApplicationIdentity());

            UserManager.DeleteAsync(Arg.Any<ApplicationIdentity>())
                .Returns(IdentityResult.Success);
        }

        [Then]
        public void ShouldNotReturnSuccess()
        {
            Assert.That(IsSuccess, Is.False);
        }

        [Then]
        public void ShouldReturnExceptionMessage()
        {
            Assert.That(ErrorMessage, Is.EqualTo("boom"));
        }

        [Then]
        public void ShouldDeleteIdentity()
        {
            UserManager.Received(1).DeleteAsync(Arg.Any<ApplicationIdentity>());
        }
    }
}
