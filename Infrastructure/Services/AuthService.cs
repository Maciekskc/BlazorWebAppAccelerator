using Data.Models;
using Infrastructure.Interfaces;
using Infrastructure.Security;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.DTOs.Auth.Requests;
using Shared.DTOs.Auth.Responses;
using System.Net;

namespace Infrastructure.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;


        public AuthService(IServiceProvider serviceProvider, SignInManager<ApplicationUser> signInManager, IJwtGenerator jwtGenerator) : base(serviceProvider)
        {
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);

            if (user == null)
                return new ServiceResponse<LoginResponse>(HttpStatusCode.Unauthorized,new string[] { "Unauthorized" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return new ServiceResponse<LoginResponse>(HttpStatusCode.Unauthorized,new string[] { "Wrong Password" });
            var generatedToken = await _jwtGenerator.CreateTokenAsync(user);

            return new ServiceResponse<LoginResponse>(HttpStatusCode.OK, new LoginResponse()
            {
                Token = generatedToken.Token,
                RefreshToken = generatedToken.RefreshToken,
            });
        }

        public async Task<ServiceResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var userToRegister = new ApplicationUser()
            {
                Initials = request.Initials,
                Email = request.Email,
                UserName = request.Initials
            };

            var result = await UserManager.CreateAsync(userToRegister, request.Password);

            var token = await _jwtGenerator.CreateTokenAsync(userToRegister);
            return new ServiceResponse<RegisterResponse>(HttpStatusCode.OK, new RegisterResponse
            {
                Token = token.Token,
                RefreshToken = token.RefreshToken
            });
        }

        public async Task<ServiceResponse<RefreshTokenResponse>> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.BadRequest,new string[] { "Empty token" });
            if (string.IsNullOrWhiteSpace(refreshToken))
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.BadRequest,new string[] { "Empty refresh token" });

            var validatedToken = _jwtGenerator.GetPrincipalFromToken(accessToken);

            if (validatedToken == null)
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.BadRequest, new string[] { "Wrong Token" });

            var storedRefreshToken =
                await DbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            var validationResult = _jwtGenerator.ValidateRefreshToken(storedRefreshToken!, validatedToken);

            if (validationResult.Any())
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.BadRequest,new string[] { $"Invalid Token, {JsonConvert.SerializeObject(validationResult)}" });
            storedRefreshToken!.Used = true;
            DbContext.RefreshTokens.Update(storedRefreshToken);

            var saveChangesResponse = await DbContext.SaveChangesAsync();
            if (saveChangesResponse < 0)
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.InternalServerError,new string[] { "Error while updating token" });
            var user = await UserManager.FindByIdAsync(_jwtGenerator.GetUserIdFromToken(validatedToken));

            if (user == null)
                return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.BadRequest,new string[] { "User not found" });
            var tokens = await _jwtGenerator.CreateTokenAsync(user);

            return new ServiceResponse<RefreshTokenResponse>(HttpStatusCode.OK, new RefreshTokenResponse
            {
                RefreshToken = tokens.RefreshToken,
                Token = tokens.Token 
            });
        }
    }
}