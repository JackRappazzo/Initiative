using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetAvailableBestiariesTests
{
    public class GivenSystemAndCustomBestiariesExist : WhenTestingGetAvailableBestiaries
    {
        private string _systemBestiaryId;
        private string _customBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(RepositoryReturnsSystemBestiaries)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldReturnBothBestiaries)
            .And(ShouldReturnBestiariesOrderedByName);

        [Given]
        public void UserIdIsSet()
        {
            UserId = NewId();
            _systemBestiaryId = NewId();
            _customBestiaryId = NewId();
        }

        [Given]
        public void RepositoryReturnsSystemBestiaries()
        {
            BestiaryRepository.GetBestariesByOwners(Arg.Any<IEnumerable<string?>>(), CancellationToken)
                .Returns(new List<BestiaryDocument>
                {
                    new BestiaryDocument { Id = _systemBestiaryId, Name = "Monster Manual", Source = "XMM" },
                    new BestiaryDocument { Id = _customBestiaryId, Name = "My Homebrew", Source = null, OwnerId = UserId }
                });
        }

        [Then]
        public void ShouldReturnBothBestiaries()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(2));
        }

        [Then]
        public void ShouldReturnBestiariesOrderedByName()
        {
            var names = Result.Select(b => b.Name).ToList();
            Assert.That(names, Is.EqualTo(names.OrderBy(n => n).ToList()));
        }
    }
}
