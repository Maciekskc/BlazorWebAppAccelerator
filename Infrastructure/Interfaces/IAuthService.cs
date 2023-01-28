using Infrastructure.Utilities;
using Shared.DTOs.Auth.Requests;
using Shared.DTOs.Auth.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request);

    Task<ServiceResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);

    Task<ServiceResponse<RefreshTokenResponse>> RefreshTokenAsync(string accessToken, string refreshToken);
}