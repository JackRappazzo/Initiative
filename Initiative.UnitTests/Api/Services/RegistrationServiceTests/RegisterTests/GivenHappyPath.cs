using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Api.Services.RegistrationServiceTests.RegisterTests
{
    public class GivenHappyPath : WhenTestingRegister
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameEmailAndPasswordAreSet)
            .And(EmailIsNotInUse)
            .And(UserCanBeCreated)
            .When(RegisterIsCalled)
            .Then(ShouldReturnSuccess)
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
            UserManager.CreateAsync(Arg.Is<InitiativeUser>(u => u.Email == Email), Password)
                .Returns(IdentityResult.Success);
        }

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(IsSuccess, Is.True);
        }

        [Then]
        public void ShouldNotReturnErrorMessage()
        {
            Assert.That(string.IsNullOrEmpty(ErrorMessage), Is.True);
        }
    }
}
