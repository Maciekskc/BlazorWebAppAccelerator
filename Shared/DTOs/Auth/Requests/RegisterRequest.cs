namespace Shared.DTOs.Auth.Requests
{
    public class RegisterRequest
    {
        public string Initials { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}