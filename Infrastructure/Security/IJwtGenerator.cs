using Data.Models;
using Domain.Models;
using Infrastructure.Security;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtGenerator
    {
        Task<JwtToken> CreateTokenAsync(ApplicationUser user);

        ClaimsPrincipal GetPrincipalFromToken(string token);

        string GetUserIdFromToken(ClaimsPrincipal validatedToken);

        public List<string> ValidateRefreshToken(RefreshToken refreshToken, ClaimsPrincipal validatedToken);
    }
}