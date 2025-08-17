using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters.Dtos;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.BestiaryServiceTests.GetAvailableBestiariesTests
{
    public class GivenHappyPath : WhenTestingGetAvailableBestiaries
    {
        private IEnumerable<GetAvailableBestiaryDto> _expectedBestiaries;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(RepositoryReturnsAvailableBestiaries)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnExpectedBestiaries)
            .And(ShouldCallRepositoryWithCorrectParameters);

        [Given]
        public void UserIdIsSet()
        {
            UserId = Guid.NewGuid().ToString();
        }

        [Given]
        public void RepositoryReturnsAvailableBestiaries()
        {
            _expectedBestiaries = new List<GetAvailableBestiaryDto>
            {
                new GetAvailableBestiaryDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Monster Manual",
                    OwnerId = null // System bestiary
                },
                new GetAvailableBestiaryDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User's Custom Bestiary",
                    OwnerId = UserId // User's bestiary
                }
            };

            BestiaryRepository.GetAvailableBestiaries(UserId, CancellationToken)
                .Returns(_expectedBestiaries);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnExpectedBestiaries()
        {
            Assert.That(Result, Is.Not.Null, "Result should not be null");
            Assert.That(Result, Is.EqualTo(_expectedBestiaries), "Should return the same bestiaries as the repository");
            Assert.That(Result.Count(), Is.EqualTo(2), "Should return 2 bestiaries");
        }

        [Then]
        public void ShouldCallRepositoryWithCorrectParameters()
        {
            BestiaryRepository.Received(1)
                .GetAvailableBestiaries(UserId, CancellationToken);
        }
    }
}