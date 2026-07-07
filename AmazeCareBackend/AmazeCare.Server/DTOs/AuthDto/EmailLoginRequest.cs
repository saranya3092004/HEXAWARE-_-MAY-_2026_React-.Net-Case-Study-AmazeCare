namespace AmazeCare.Server.Modules.Auth.DTOs
{
    public class EmailLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
