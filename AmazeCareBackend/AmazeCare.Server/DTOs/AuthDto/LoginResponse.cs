namespace AmazeCare.Server.DTOs
{
    public class LoginResponse
    {
        public string Token {get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? RoleSpecificId { get; set; }

    }
}
