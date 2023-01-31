using Shared.DTOs.Auth.Requests;

namespace Application.Interfaces;

public interface IAuthService
{
    Task LoginAsync(LoginRequest request);

    Task RegisterAsync(RegisterRequest request);

    Task RefreshTokenAsync(string accessToken, string refreshToken);
}