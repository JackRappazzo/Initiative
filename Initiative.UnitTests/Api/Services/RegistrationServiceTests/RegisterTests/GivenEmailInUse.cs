using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.UserServiceTests.RegisterTests
{
    public class GivenEmailInUse : WhenTestingRegister
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameEmailAndPasswordAreSet)
            .And(EmailIsAlreadyInUse)
            .When(RegisterIsCalled)
            .Then(ShouldNotReturnSuccess)
            .And(ShouldReturnEmailExistsError)
            .And(ShouldNotAttemptToCreateIdentity)
            .And(ShouldNotInsertUser);

        [Given]
        public void NameEmailAndPasswordAreSet()
        {
            DisplayName = "TestUser";
            Email = "email@address.com";
            Password = "password";
        }

        [Given]
        public void EmailIsAlreadyInUse()
        {
            UserManager.FindByEmailAsync(Email)
                .Returns(new ApplicationIdentity());
        }

        [Then]
        public void ShouldNotReturnSuccess()
        {
            Assert.That(IsSuccess, Is.False);
        }

        [Then]
        public void ShouldReturnEmailExistsError()
        {
            Assert.That(ErrorMessage, Is.EqualTo("Email exists"));
        }

        [Then]
        public void ShouldNotAttemptToCreateIdentity()
        {
            UserManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationIdentity>(), Arg.Any<string>());
        }

        [Then]
        public void ShouldNotInsertUser()
        {
            UserRepository.DidNotReceive().InsertUser(Arg.Any<Initiative.Persistence.Models.InitiativeUserModel>(), CancellationToken);
        }
    }
}
