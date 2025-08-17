using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters.Dtos;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Services.BestiaryServiceTests.GetAvailableBestiariesTests
{
    public abstract class WhenTestingGetAvailableBestiaries : WhenTestingBestiaryService
    {
        protected string UserId;
        protected IEnumerable<GetAvailableBestiaryDto> Result;

        [When]
        public async Task GetAvailableBestiariesIsCalled()
        {
            Result = await BestiaryService.GetAvailableBestiaries(UserId, CancellationToken);
        }
    }
}