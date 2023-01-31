using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Shared.DTOs.Auth.Requests;

namespace Application.Services;

public class AuthService : BaseService, IAuthService
{
    public AuthService(IConfiguration configuration) : base(configuration)
    {
    }

    Task IAuthService.LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    Task IAuthService.RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    Task IAuthService.RefreshTokenAsync(string accessToken, string refreshToken)
    {
        throw new NotImplementedException();
    }
}