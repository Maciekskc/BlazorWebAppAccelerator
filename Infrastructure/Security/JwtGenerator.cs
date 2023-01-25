using Data.Models;
using Domain.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class JwtGenerator : BaseService, IJwtGenerator
    {
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public JwtGenerator(IServiceProvider serviceProvider, IConfiguration config, RoleManager<IdentityRole> roleManager):base(serviceProvider) 
        {
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<JwtToken> CreateTokenAsync(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Initials),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id),
            };

            var userClaims = await UserManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await UserManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var role = await _roleManager.FindByNameAsync(userRole);

                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;
                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiry"])),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:RefreshTokenExpiry"])),
            };

            await DbContext.RefreshTokens.AddAsync(refreshToken);
            await DbContext.SaveChangesAsync();

            return new JwtToken
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal =
                    tokenHandler.ValidateToken(token, TokenValidationParametersDefaults.GetDefaultParameters(),
                        out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> ValidateRefreshToken(RefreshToken refreshToken, ClaimsPrincipal validatedToken)
        {
            var errors = new List<string>();

            if (refreshToken == null)
            {
                errors.Add("Token does not exist");
                return errors;
            }

            if (DateTime.UtcNow > refreshToken.ExpiryDate)
            {
                errors.Add("Token Expired");
            }

            if (refreshToken.Invalidated)
            {
                errors.Add("Invalid Token");
            }

            if (refreshToken.Used)
            {
                errors.Add("Used Token");
            }

            var jti = GetJtiFromToken(validatedToken);

            if (refreshToken.JwtId != jti)
            {
                errors.Add("Given tocken does not match this JWT");
            }

            return errors;
        }

        public string GetUserIdFromToken(ClaimsPrincipal validatedToken)
        {
            return validatedToken.Claims.Single(x => x.Type == "userId").Value;
        }

        private string GetJtiFromToken(ClaimsPrincipal validatedToken)
        {
            return validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        }
    }
}