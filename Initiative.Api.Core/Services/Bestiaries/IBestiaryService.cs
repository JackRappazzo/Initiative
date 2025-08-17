using Initiative.Persistence.Models.Encounters.Dtos;

namespace Initiative.Api.Core.Services.Bestiaries
{
    public interface IBestiaryService
    {
        Task<IEnumerable<GetAvailableBestiaryDto>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken);
    }
}