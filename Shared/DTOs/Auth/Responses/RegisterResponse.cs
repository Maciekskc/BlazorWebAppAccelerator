namespace Shared.DTOs.Auth.Responses
{
    public class RegisterResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}