using Initiative.Persistence.Models.Encounters.Dtos;
using Initiative.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Api.Core.Services.Bestiaries
{
    public class BestiaryService : IBestiaryService
    {
        IBestiaryRepository bestiaryRepository;

        public BestiaryService(IBestiaryRepository bestiaryRepository)
        {
            this.bestiaryRepository = bestiaryRepository;
        }

        public async Task<IEnumerable<GetAvailableBestiaryDto>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken)
        {
            return await bestiaryRepository.GetAvailableBestiaries(userId, cancellationToken);
        }
    }
}
