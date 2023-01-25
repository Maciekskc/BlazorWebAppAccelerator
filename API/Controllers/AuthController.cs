using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Auth.Requests;
using Shared.DTOs.Auth.Responses;
using System.Net;

namespace API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [Produces(typeof(LoginResponse))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var response = await _authService.LoginAsync(dto);

            AssignTokenCookiesToResponse(response.Token, response.RefreshToken);

            return Ok(response);
        }

        [Produces(typeof(RegisterResponse))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        [Produces(typeof(RefreshTokenResponse))]
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            HttpContext.Request.Cookies.TryGetValue("refresh-token", out var refreshToken);
            HttpContext.Request.Cookies.TryGetValue("access-token", out var accessToken);

            var response = await _authService.RefreshTokenAsync(accessToken, refreshToken);

            AssignTokenCookiesToResponse(response.Token, response.RefreshToken);

            return Ok(response);
        }

        private void AssignTokenCookiesToResponse(string accessToken, string refreshToken)
        {
            var expiryOffset = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:RefreshTokenExpiry"));

            HttpContext.Response.Cookies.Append("access-token", accessToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Expires = expiryOffset
            });

            HttpContext.Response.Cookies.Append("refresh-token", refreshToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Expires = expiryOffset
            });
        }
    }
}