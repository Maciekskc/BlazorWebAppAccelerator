using Shared.DTOs.Auth.Requests;
using Shared.DTOs.Auth.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken);
}