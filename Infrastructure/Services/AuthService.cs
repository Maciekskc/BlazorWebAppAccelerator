using Data.Models;
using Infrastructure.Interfaces;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.DTOs.Auth.Requests;
using Shared.DTOs.Auth.Responses;
using System.ComponentModel.DataAnnotations;
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

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new HttpRequestException($"Unauthorized", null, HttpStatusCode.Unauthorized);


            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                throw new HttpRequestException($"Wrong Password", null, HttpStatusCode.Unauthorized);

            var generatedToken = await _jwtGenerator.CreateTokenAsync(user);

            return new LoginResponse()
            {
                Token = generatedToken.Token,
                RefreshToken = generatedToken.RefreshToken,
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var userToRegister = new ApplicationUser()
            {
                Initials = request.Initials,
                Email = request.Email,
                UserName = request.Initials
            };

            await UserManager.CreateAsync(userToRegister, request.Password);

            var token = await _jwtGenerator.CreateTokenAsync(userToRegister);
            return new RegisterResponse
            {
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new HttpRequestException("Empty token",null, HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new HttpRequestException("Empty refresh token", null, HttpStatusCode.BadRequest);


            var validatedToken = _jwtGenerator.GetPrincipalFromToken(accessToken);

            if (validatedToken == null)
                throw new HttpRequestException("Wrong Token", null, HttpStatusCode.BadRequest);


            var storedRefreshToken =
                await DbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            var validationResult = _jwtGenerator.ValidateRefreshToken(storedRefreshToken, validatedToken);

            if (validationResult.Any())
                throw new HttpRequestException($"Invalid Token, {JsonConvert.SerializeObject(validationResult)}", null, HttpStatusCode.BadRequest);

            storedRefreshToken.Used = true;
            DbContext.RefreshTokens.Update(storedRefreshToken);

            var saveChangesResponse = await DbContext.SaveChangesAsync();
            if (saveChangesResponse < 0)
                throw new HttpRequestException("Error while updating token", null, HttpStatusCode.InternalServerError);

            var user = await UserManager.FindByIdAsync(_jwtGenerator.GetUserIdFromToken(validatedToken));

            if (user == null)
                throw new HttpRequestException("User not found", null, HttpStatusCode.BadRequest);

            var tokens = await _jwtGenerator.CreateTokenAsync(user);

            return new RefreshTokenResponse { RefreshToken = tokens.RefreshToken, Token = tokens.Token };
        }
    }
}