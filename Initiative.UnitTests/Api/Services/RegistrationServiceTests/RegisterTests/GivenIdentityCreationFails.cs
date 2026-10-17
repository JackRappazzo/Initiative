using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Api.Services.UserServiceTests.RegisterTests
{
    public class GivenIdentityCreationFails : WhenTestingRegister
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameEmailAndPasswordAreSet)
            .And(EmailIsNotInUse)
            .And(IdentityCreationFails)
            .When(RegisterIsCalled)
            .Then(ShouldNotReturnSuccess)
            .And(ShouldReturnCombinedValidationErrors)
            .And(ShouldNotInsertUser);

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
        public void IdentityCreationFails()
        {
            UserManager.CreateAsync(Arg.Is<ApplicationIdentity>(u => u.Email == Email), Password)
                .Returns(IdentityResult.Failed(
                    new IdentityError { Code = "PasswordTooShort", Description = "Passwords must be at least 6 characters." },
                    new IdentityError { Code = "PasswordRequiresUpper", Description = "Passwords must have at least one uppercase ('A'-'Z')." }));
        }

        [Then]
        public void ShouldNotReturnSuccess()
        {
            Assert.That(IsSuccess, Is.False);
        }

        [Then]
        public void ShouldReturnCombinedValidationErrors()
        {
            Assert.That(ErrorMessage, Is.EqualTo("Passwords must be at least 6 characters. Passwords must have at least one uppercase ('A'-'Z')."));
        }

        [Then]
        public void ShouldNotInsertUser()
        {
            UserRepository.DidNotReceive().InsertUser(Arg.Any<Initiative.Persistence.Models.InitiativeUserModel>(), CancellationToken);
        }
    }
}
