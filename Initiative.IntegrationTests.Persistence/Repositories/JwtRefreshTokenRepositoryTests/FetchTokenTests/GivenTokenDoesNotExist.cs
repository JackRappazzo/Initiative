using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests.FetchTokenTests
{
    public class GivenTokenDoesNotExist : WhenTestingFetchToken
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(TokenDoesNotExist)
            .When(FetchTokenIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void TokenDoesNotExist()
        {
            RefreshToken = "invalid-token";

            //Do nothing else
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}
