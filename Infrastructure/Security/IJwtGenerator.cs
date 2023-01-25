using Data.Models;
using Domain.Models;
using System.Security.Claims;

namespace Infrastructure.Security
{
    public interface IJwtGenerator
    {
        Task<JwtToken> CreateTokenAsync(ApplicationUser user);

        ClaimsPrincipal GetPrincipalFromToken(string token);

        string GetUserIdFromToken(ClaimsPrincipal validatedToken);

        public List<string> ValidateRefreshToken(RefreshToken refreshToken, ClaimsPrincipal validatedToken);
    }
}