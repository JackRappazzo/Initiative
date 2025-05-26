using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Initiative.Api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
