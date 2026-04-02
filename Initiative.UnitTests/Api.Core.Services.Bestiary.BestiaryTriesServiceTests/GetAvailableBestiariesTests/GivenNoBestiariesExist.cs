using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetAvailableBestiariesTests
{
    public class GivenNoBestiariesExist : WhenTestingGetAvailableBestiaries
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(RepositoryReturnsEmpty)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldReturnEmptyCollection);

        [Given]
        public void UserIdIsSet()
        {
            UserId = NewId();
        }

        [Given]
        public void RepositoryReturnsEmpty()
        {
            BestiaryRepository.GetBestariesByOwners(Arg.Any<IEnumerable<string?>>(), CancellationToken)
                .Returns(Enumerable.Empty<BestiaryDocument>());
        }

        [Then]
        public void ShouldReturnEmptyCollection()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Empty);
        }
    }
}
